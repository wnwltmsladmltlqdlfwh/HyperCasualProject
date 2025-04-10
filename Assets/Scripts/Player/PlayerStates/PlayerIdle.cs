using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerIdle : IState<PlayerController>
{
    private PlayerController _playerController;
    public void OperatorEnter(PlayerController sender)
    {
        _playerController = sender;
        _playerController.CurrentSpeed = 0f;

        _playerController.animator.SetBool("isRun", false);
        _playerController.animator.SetFloat("isSpeed", 0f);
        _playerController.isMoving = false;
    }

    public void OperatorUpdate(PlayerController sender)
    {
        
    }

    public void OperatorExit(PlayerController sender)
    {
        
    }
}
