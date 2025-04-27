using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                _customer.animator.SetFloat("isSpeed", _customer.navMeshAgent.velocity.magnitude);

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

        float duration = 0f;

        public void OperatorEnter(Customer sender)
        {
            _customer = sender;
            findDisplayTable = InteractedObjectManager.Instance.displayTableDict[_customer.needItemType];
            findDisplayTable.customersQueue.Enqueue(_customer);

            targetPos = findDisplayTable.gameObject.transform.position + new Vector3(0f, 0f, -1f - (findDisplayTable.customersQueue.Count * 1f));

            _customer.navMeshAgent.SetDestination(targetPos);
        }

        public void OperatorUpdate(Customer sender)
        {
            if (_customer)
            {
                _customer.animator.SetFloat("isSpeed", _customer.navMeshAgent.velocity.magnitude);

                if (!_customer.navMeshAgent.pathPending && _customer.navMeshAgent.remainingDistance <= _customer.navMeshAgent.stoppingDistance)
                {
                    if (!_customer.navMeshAgent.hasPath || _customer.navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        _customer.transform.LookAt(findDisplayTable.transform);

                        if (_customer.shoppingTrayStack.Count >= _customer.needItemCapacity)
                        {
                            duration += Time.deltaTime;
                            if (duration >= 1f)
                            {
                                findDisplayTable.customersQueue.Dequeue();
                                findDisplayTable.UpdateCustomersQueue();
                                _customer.stateMachine.SetState(_customer.dictionaryState[Customer.CustomerState.StandInLine]);
                                duration = 0f;
                            }
                        }

                        if (findDisplayTable.customersQueue.Count > 0
                            && findDisplayTable.customersQueue.Peek() == _customer
                            && _customer.shoppingTrayStack.Count < _customer.needItemCapacity
                            && findDisplayTable.displayItems.Count > 0)
                        {
                            duration += Time.deltaTime;
                            if (duration >= 0.5f)
                            {
                                findDisplayTable.GiveItemToCustomer(_customer, _customer.jumpPoint.position);
                                duration = 0f;
                            }
                        }
                    }
                }
            }
        }

        public void OperatorExit(Customer sender)
        {

        }
    }

    /// <summary>
    /// 구매 줄 대기
    /// </summary>
    public class StandInLine : IState<Customer>
    {
        Customer _customer;
        Vector3 targetPos;
        CashierCounter findCounter;
        bool hasQueueOnce;

        public void OperatorEnter(Customer sender)
        {
            _customer = sender;

            findCounter = InteractedObjectManager.Instance.cashierCounter;

            targetPos = findCounter.gameObject.transform.position + new Vector3(0f, 0f, 1f + ((findCounter.payCustomersQueue.Count + 1) * 1.5f));

            hasQueueOnce = false;

            _customer.navMeshAgent.SetDestination(targetPos);
        }

        public void OperatorUpdate(Customer sender)
        {
            if (_customer)
            {
                _customer.animator.SetFloat("isSpeed", _customer.navMeshAgent.velocity.magnitude);

                if (!_customer.navMeshAgent.pathPending && _customer.navMeshAgent.remainingDistance <= _customer.navMeshAgent.stoppingDistance)
                {
                    if (!_customer.navMeshAgent.hasPath || _customer.navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        _customer.transform.LookAt(findCounter.transform);

                        if (!hasQueueOnce && findCounter.payCustomersQueue.Contains(_customer) == false)
                        {
                            findCounter.payCustomersQueue.Enqueue(_customer);
                            findCounter.UpdatePayCustomersQueue();
                            hasQueueOnce = true;
                        }

                        if (_customer.shoppingTrayStack.Count <= 0)
                        {
                            switch (_customer.eatInShop)
                            {
                                case true:
                                    _customer.stateMachine.SetState(_customer.dictionaryState[Customer.CustomerState.WaitSeat]);
                                    break;
                                case false:
                                    _customer.stateMachine.SetState(_customer.dictionaryState[Customer.CustomerState.GoHome]);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void OperatorExit(Customer sender)
        {

        }
    }

    /// <summary>
    /// 귀가
    /// </summary>
    public class GoHome : IState<Customer>
    {
        Customer _customer;
        Vector3 targetPos;

        public void OperatorEnter(Customer sender)
        {
            _customer = sender;
            targetPos = CustomerManager.Instance.customerEnterPos.position;
            _customer.navMeshAgent.SetDestination(targetPos);
        }

        public void OperatorUpdate(Customer sender)
        {
            if (_customer)
            {
                _customer.animator.SetFloat("isSpeed", _customer.navMeshAgent.velocity.magnitude);

                if (!_customer.navMeshAgent.pathPending && _customer.navMeshAgent.remainingDistance <= _customer.navMeshAgent.stoppingDistance)
                {
                    if (!_customer.navMeshAgent.hasPath || _customer.navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        CustomerManager.Instance.ReturnNPC(_customer);
                    }
                }
            }
        }

        public void OperatorExit(Customer sender)
        {

        }
    }

    /// <summary>
    /// 좌석 대기
    /// </summary>
    public class WaitSeat : IState<Customer>
    {
        Customer _customer;
        Vector3 targetPos;
        CashierCounter findCounter;
        bool hasQueueOnce;

        int debugCount = 0;

        public void OperatorEnter(Customer sender)
        {
            _customer = sender;
            findCounter = InteractedObjectManager.Instance.cashierCounter;
            targetPos = findCounter.transform.position + new Vector3(2f + ((findCounter.payCustomersQueue.Count + 1) * 1.5f), 0f, 0f);
            _customer.navMeshAgent.SetDestination(targetPos);
            hasQueueOnce = false;
        }

        public void OperatorUpdate(Customer sender)
        {
            if (_customer)
            {
                _customer.animator.SetFloat("isSpeed", _customer.navMeshAgent.velocity.magnitude);

                if (!_customer.navMeshAgent.pathPending && _customer.navMeshAgent.remainingDistance <= _customer.navMeshAgent.stoppingDistance)
                {
                    if (!_customer.navMeshAgent.hasPath || _customer.navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        _customer.transform.LookAt(InteractedObjectManager.Instance.cashierCounter.transform);

                        if (findCounter.payCustomersQueue.Contains(_customer) == false)
                        {
                            if (!hasQueueOnce)
                            {
                                findCounter.dineInCustomersQueue.Enqueue(_customer);
                                findCounter.UpdateDineInQueue();
                                hasQueueOnce = true;
                            }
                            else
                            {
                                var dineInTable = InteractedObjectManager.Instance.GetEmptyDineInTable();
                                if (dineInTable != null)
                                {
                                    _customer.stateMachine.SetState(_customer.dictionaryState[Customer.CustomerState.Eating]);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OperatorExit(Customer sender)
        {
            findCounter.dineInCustomersQueue.Dequeue();
            findCounter.UpdateDineInQueue();
        }
    }

    /// <summary>
    /// 식사
    /// </summary>
    public class Eating : IState<Customer>
    {
        Customer _customer;
        Vector3 targetPos;
        DineInTable findDineInTable;

        bool isSeatOnce;

        float duration = 0f;

        public void OperatorEnter(Customer sender)
        {
            _customer = sender;

            findDineInTable = InteractedObjectManager.Instance.GetEmptyDineInTable();
            if (findDineInTable == null)
                return;

            findDineInTable.TableStates = DineInTable.DineInTableState.Reserved;

            isSeatOnce = false;

            targetPos = findDineInTable.transform.position + new Vector3(-1f, 0f, 0f);
            _customer.navMeshAgent.SetDestination(targetPos);
        }

        public void OperatorUpdate(Customer sender)
        {

            if (_customer)
            {
                _customer.animator.SetFloat("isSpeed", _customer.navMeshAgent.velocity.magnitude);
                
                if (isSeatOnce == true)
                {
                    duration += Time.deltaTime;

                    if (duration > 5f)
                    {
                        _customer.transform.position = findDineInTable.transform.position + new Vector3(-1f, 0f, 0f);
                        _customer.stateMachine.SetState(_customer.dictionaryState[Customer.CustomerState.GoHome]);
                        duration = 0f;
                    }
                }
                else
                {
                    if (!_customer.navMeshAgent.pathPending && _customer.navMeshAgent.remainingDistance <= _customer.navMeshAgent.stoppingDistance)
                    {
                        if (!_customer.navMeshAgent.hasPath || _customer.navMeshAgent.velocity.sqrMagnitude == 0f)
                        {
                            _customer.animator.SetTrigger("isSit");
                            _customer.navMeshAgent.isStopped = true;
                            _customer.transform.position = findDineInTable.seatChairTransform.position + new Vector3(0f, 0.35f, 0f);
                            _customer.transform.localRotation = Quaternion.identity;
                            findDineInTable.TableStates = DineInTable.DineInTableState.Occupied;
                            findDineInTable.customerNeedItemCapacity = _customer.needItemCapacity;
                            isSeatOnce = true;
                        }
                    }
                }
            }
        }

        public void OperatorExit(Customer sender)
        {
            _customer.navMeshAgent.isStopped = false;
            findDineInTable.TableStates = DineInTable.DineInTableState.NeedCleaning;
            _customer.animator.SetTrigger("isStandUp");
        }
    }
}
