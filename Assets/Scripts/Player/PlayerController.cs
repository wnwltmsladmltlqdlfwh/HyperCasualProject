using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.AI;

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

    private void Awake()
    {
        if(joystickController == null)
            joystickController = FindObjectOfType<JoystickController>();

        if(navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
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

        if(MoveDir != Vector3.zero)
            stateMachine.SetState(dictionaryState[PlayerState.Move]);

        stateMachine.DoOperaterUpdate();
    }
}
