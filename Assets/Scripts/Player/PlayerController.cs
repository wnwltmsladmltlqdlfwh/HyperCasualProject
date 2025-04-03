using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move
    }

    public float MaxSpeed = 2.0f;
    public float turnDistance = 2.0f;

    public Vector3 MoveDir = Vector3.zero;

    public float CurrentSpeed { get; set; }

    public TurnDirection CurrentTurnDirection { get; private set; }
    public enum TurnDirection
    {
        Left = -1,
        Right = 1,
    }

    private StateMachineBase<PlayerController> stateMachine;
    private Dictionary<PlayerState, IState<PlayerController>> dictionaryState = new Dictionary<PlayerState, IState<PlayerController>>();

    public JoystickController joystickController;

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

    }
}
