using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move
    }

    public float MaxSpeed = 2.0f;
    public float turnDistance = 2.0f;
    public Vector3 MoveDir;

    public float CurrentSpeed { get; set; }

    private StateMachineBase<PlayerController> stateMachine;
    private Dictionary<PlayerState, IState<PlayerController>> dictionaryState = new Dictionary<PlayerState, IState<PlayerController>>();
    public bool isMoving = false;

    public JoystickController joystickController;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    public List<MerchandiseItem> objectStack = new List<MerchandiseItem>();
    public int objectCapacity = 10;

    [SerializeField]
    private GameObject trayObject;
    public Transform overTray;

    private void Awake()
    {
        if (joystickController == null)
            joystickController = FindObjectOfType<JoystickController>();

        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        IState<PlayerController> idleState = new PlayerIdle();
        IState<PlayerController> moveState = new PlayerMove();

        dictionaryState.Add(PlayerState.Idle, idleState);
        dictionaryState.Add(PlayerState.Move, moveState);

        stateMachine = new StateMachineBase<PlayerController>(this, dictionaryState[PlayerState.Idle]);
    }

    private void Update()
    {
        MoveDir = joystickController?.GetMoveDirection() ?? Vector3.zero;

        if (MoveDir != Vector3.zero)
        {
            if (stateMachine.CurrentState != dictionaryState[PlayerState.Move])
                stateMachine.SetState(dictionaryState[PlayerState.Move]);
        }
        else
        {
            if (stateMachine.CurrentState != dictionaryState[PlayerState.Idle])
                stateMachine.SetState(dictionaryState[PlayerState.Idle]);
        }

        stateMachine.DoOperaterUpdate();
    }

    public void AddObjectList(MerchandiseItem item)
    {
        objectStack.Add(item);

        item.transform.SetParent(overTray);

        item.transform.localPosition = new Vector3(0f, (item.topY * 2f * (objectStack.Count - 1)) - 0.6f, 0f);

        item.transform.localRotation = Quaternion.identity;

        item.TurnOnPhysics(false);

        trayObject.SetActive(objectStack.Count > 0);
        animator.SetInteger("isCarryObjects", objectStack.Count);
    }

    public MerchandiseItem FindObjectInList(ItemType findType)
    {
        for (int i = objectStack.Count - 1; i >= 0; i--)
        {
            if (objectStack[i].itemType == findType)
            {
                MerchandiseItem findItem = objectStack[i];

                for (int j = i; j < objectStack.Count; j++)
                {
                    Vector3 targetPos = objectStack[j].transform.localPosition;
                    float itemHeight = objectStack[i].GetComponent<Collider>().bounds.size.y;
                    targetPos.y -= itemHeight;
                    objectStack[j].transform.DOLocalMove(targetPos, 0.2f).SetEase(Ease.Linear);
                }
                objectStack.RemoveAt(i);

                trayObject.SetActive(objectStack.Count > 0);
                animator.SetInteger("isCarryObjects", objectStack.Count);
                
                return findItem;
            }
        }

        return null;
    }

    public void RemovedStack()
    {
        /*
        ItemManager.Instance.ReturnItem(item.itemType.ToString(), item);

        trayObject.SetActive(objectStack.Count > 0);
        animator.SetInteger("isCarryObjects", objectStack.Count);
        */
    }
}
