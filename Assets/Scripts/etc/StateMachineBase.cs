using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBase<T>
{
    private T m_sender;

    public IState<T> CurState { get; set; }

    public StateMachineBase(T sender, IState<T> state)
    {
        m_sender = sender;
    }

    public void SetState(IState<T> state)
    {
        if(m_sender == null || CurState == state || CurState == null)
            return;

        if(CurState != null)
            CurState.OperatorExit(m_sender);
        
        CurState = state;

        if(m_sender != null)
        {
            CurState.OperatorEnter(m_sender);
        }
    }

    public void DoOperaterUpdate()
    {
        if (m_sender == null)
            return;

        CurState.OperatorUpdate(m_sender);
    }
}
