using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomerState
{
    /// <summary>
    /// 입장
    /// </summary>
    public class Enter : IState<Customer>
    {
        Customer _customer;
        public void OperatorEnter(Customer sender)
        {
            _customer = sender;
            _customer.navMeshAgent.SetDestination(CustomerManager.Instance.customerEnterPos.position);
        }

        public void OperatorUpdate(Customer sender)
        {
            if (_customer)
            {
                if (!_customer.navMeshAgent.pathPending && _customer.navMeshAgent.remainingDistance <= _customer.navMeshAgent.stoppingDistance)
                {
                    if (!_customer.navMeshAgent.hasPath || _customer.navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        _customer.stateMachine.SetState(_customer.dictionaryState[Customer.CustomerState.WaitRestock]);
                    }
                }
            }
        }

        public void OperatorExit(Customer sender)
        {

        }
    }

    /// <summary>
    /// 상품 재입고 대기 (입고 시 바로 구매 후 줄서기)
    /// </summary>
    public class WaitRestock : IState<Customer>
    {
        Customer _customer;
        DisplayTable findDisplayTable;
        Vector3 targetPos;

        public void OperatorEnter(Customer sender)
        {
            _customer = sender;
            findDisplayTable = CustomerManager.Instance.displayTableDict[_customer.needItemType];
            findDisplayTable.customersQueue.Enqueue(_customer);

            targetPos = findDisplayTable.gameObject.transform.position + new Vector3(0f, 0f, -1f -(findDisplayTable.customersQueue.Count * 1f));

            _customer.navMeshAgent.SetDestination(targetPos);
        }

        public void OperatorUpdate(Customer sender)
        {
            if (_customer)
            {
                if (!_customer.navMeshAgent.pathPending && _customer.navMeshAgent.remainingDistance <= _customer.navMeshAgent.stoppingDistance)
                {
                    if (!_customer.navMeshAgent.hasPath || _customer.navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        _customer.transform.LookAt(findDisplayTable.transform);
                    }
                }
            }
        }

        public void OperatorExit(Customer sender)
        {
            
        }

        public void TakeItems()
        {
            findDisplayTable.customersQueue.Dequeue();

            findDisplayTable.UpdateCustomersQueue();

            _customer.stateMachine.SetState(_customer.dictionaryState[Customer.CustomerState.StandInLine]);
        }
    }

    /// <summary>
    /// 구매 줄 대기
    /// </summary>
    public class StandInLine : IState<Customer>
    {
        Customer _customer;

        public void OperatorEnter(Customer sender)
        {
            _customer = sender;
        }

        public void OperatorUpdate(Customer sender)
        {
            
        }

        public void OperatorExit(Customer sender)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// 귀가가
    /// </summary>
    public class GoHome : IState<Customer>
    {
        public void OperatorEnter(Customer sender)
        {
            throw new System.NotImplementedException();
        }

        public void OperatorUpdate(Customer sender)
        {
            throw new System.NotImplementedException();
        }

        public void OperatorExit(Customer sender)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// 좌석 대기
    /// </summary>
    public class WaitSeat : IState<Customer>
    {
        public void OperatorEnter(Customer sender)
        {
            throw new System.NotImplementedException();
        }

        public void OperatorUpdate(Customer sender)
        {
            throw new System.NotImplementedException();
        }

        public void OperatorExit(Customer sender)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// 식사
    /// </summary>
    public class Eating : IState<Customer>
    {
        public void OperatorEnter(Customer sender)
        {
            throw new System.NotImplementedException();
        }

        public void OperatorUpdate(Customer sender)
        {
            throw new System.NotImplementedException();
        }

        public void OperatorExit(Customer sender)
        {
            throw new System.NotImplementedException();
        }
    }
}
