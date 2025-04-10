using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    public enum CustomerState
    {
        Enter,
        WaitRestock,
        StandInLine,
        GoHome,
        WaitSeat,
        Eating,
    }

    public StateMachineBase<Customer> stateMachine { get; private set; }
    public Dictionary<CustomerState, IState<Customer>> dictionaryState { get; private set; }

    public NavMeshAgent navMeshAgent;
    public Animator animator;


    [SerializeField]
    private GameObject trayObject;
    public Transform overTray;
    public Stack<MerchandiseItem> shoppingTray = new Stack<MerchandiseItem>();
    public ItemType needItemType { get; private set; }
    int needItemCount;

    void Start()
    {
        InitCustomer();
    }

    public void InitCustomer()
    {
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        OnInitDictionary();

        int setRandomItemType = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ItemType)).Length);

        needItemType = (ItemType)setRandomItemType;
        needItemCount = UnityEngine.Random.Range(1, 3);

        stateMachine = new StateMachineBase<Customer>(this, dictionaryState[CustomerState.Enter]);
    }

    private void OnInitDictionary()
    {
        if (dictionaryState == null)
            dictionaryState = new Dictionary<CustomerState, IState<Customer>>();

        foreach (CustomerState keyState in System.Enum.GetValues(typeof(CustomerState)))
        {
            if (dictionaryState.ContainsKey(keyState)) continue;

            string className = keyState.ToString();
            string withNameSpace = $"CustomerState.{className}";

            System.Type type = System.Type.GetType(withNameSpace);

            if (type == null)
            {
                Debug.LogError($"해당 클래스는 존재하지 않습니다. : {withNameSpace}");
                continue;
            }

            IState<Customer> instanceState = (IState<Customer>)System.Activator.CreateInstance(type);

            dictionaryState.Add(keyState, instanceState);
        }
    }

    void Update()
    {
        if (stateMachine.CurrentState == dictionaryState[CustomerState.Enter])
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    stateMachine.SetState(dictionaryState[CustomerState.WaitRestock]);
                }
            }
        }
        else if (stateMachine.CurrentState == dictionaryState[CustomerState.WaitRestock])
        {

        }

        stateMachine.DoOperaterUpdate();
    }
}
