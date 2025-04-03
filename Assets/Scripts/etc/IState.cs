using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region FSM
public interface IState<T>
{
    public void OperatorEnter(T sender);
    public void OperatorUpdate(T sender);
    public void OperatorExit(T sender);
}
#endregion
