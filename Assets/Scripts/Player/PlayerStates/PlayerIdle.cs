using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerIdle : MonoBehaviour, IState<PlayerController>
{
    private PlayerController _playerController;
    public void OperatorEnter(PlayerController sender)
    {
        _playerController = sender;
        _playerController.CurrentSpeed = 0f;
    }

    public void OperatorUpdate(PlayerController sender)
    {
        
    }

    public void OperatorExit(PlayerController sender)
    {
        
    }
}
