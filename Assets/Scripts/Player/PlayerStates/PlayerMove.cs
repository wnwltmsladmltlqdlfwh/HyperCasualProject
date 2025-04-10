using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : IState<PlayerController>
{
    private PlayerController _playerController;
    public void OperatorEnter(PlayerController sender)
    {
        _playerController = sender;
        _playerController.CurrentSpeed = _playerController.MaxSpeed;
        _playerController.animator.SetBool("isRun", true);
        _playerController.isMoving = true;
    }

    public void OperatorUpdate(PlayerController sender)
    {
        if (_playerController)
        {
            if (_playerController.CurrentSpeed > 0)
            {
                _playerController.animator.SetFloat("isSpeed", 1f);
                _playerController.navMeshAgent.SetDestination(_playerController.transform.position + _playerController.MoveDir);
            }
        }
    }

    public void OperatorExit(PlayerController sender)
    {

    }
}
