using System.Collections;
using System.Collections.Generic;
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

    public JoystickController joystickController;
    public NavMeshAgent navMeshAgent;
    public Animator animator;

    public Stack<MerchandiseItem> objectStack = new Stack<MerchandiseItem>();
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

    public void PushObjectStack(MerchandiseItem item)
    {
        objectStack.Push(item);
        item.GetComponent<Rigidbody>().useGravity = false;
        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
        item.GetComponent<Collider>().enabled = false;
        item.transform.localPosition = new Vector3(0f, objectStack.Count, 0f);
        item.transform.localRotation = Quaternion.identity;
        
        trayObject.SetActive(objectStack.Count > 0);
        animator.SetInteger("isCarryObjects", objectStack.Count);
    }

    public void PopObjectStack(Transform parent)
    {
        if(objectStack.Count == 0) return;
        
        var item = objectStack.Pop();
        item.transform.SetParent(parent);

        trayObject.SetActive(objectStack.Count > 0);
        animator.SetInteger("isCarryObjects", objectStack.Count);
    }

    public void RemovedStack()
    {
        var item = objectStack.Pop();

        ItemManager.Instance.ReturnItem(item.itemType.ToString(), item);

        trayObject.SetActive(objectStack.Count > 0);
        animator.SetInteger("isCarryObjects", objectStack.Count);
    }
}
