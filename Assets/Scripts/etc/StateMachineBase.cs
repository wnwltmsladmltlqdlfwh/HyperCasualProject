using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBase<T>
{
    private T m_sender;

    public IState<T> CurrentState { get; set; }

    public StateMachineBase(T sender, IState<T> state)
    {
        m_sender = sender;
        SetState(state);
    }

    public void SetState(IState<T> state)
    {
        if (m_sender == null)
        {
            Debug.LogError("Sender is null. On SetState");
            return;
        }

        if (CurrentState == state)
        {
            Debug.LogWarningFormat("State is same. : ", state);
            return;
        }

        if (CurrentState != null)
            CurrentState.OperatorExit(m_sender);

        CurrentState = state;

        if (CurrentState != null)
        {
            CurrentState.OperatorEnter(m_sender);
        }

        Debug.Log("SetNextState : " + state);
    }

    public void DoOperaterUpdate()
    {
        if (m_sender == null)
        {
            Debug.LogError("Sender is null. On Update");
            return;
        }

        CurrentState.OperatorUpdate(m_sender);
    }
}
