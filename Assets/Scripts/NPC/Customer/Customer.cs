using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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


    public GameObject trayObject;
    public Transform jumpPoint;
    public Stack<MerchandiseItem> shoppingTrayStack = new Stack<MerchandiseItem>();
    public ItemType needItemType { get; private set; }
    public int needItemCapacity;

    public bool eatInShop = false;

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
        //needItemCapacity = Random.Range(1, 3);
        needItemCapacity = 2;
        eatInShop = Random.Range(0, 2) == 1 ? true : false;

        shoppingTrayStack.Clear();
        trayObject.SetActive(false);

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
        if (stateMachine == null)
            return;

        stateMachine.DoOperaterUpdate();
    }

    public void ReceiveItem(MerchandiseItem item)
    {
        shoppingTrayStack.Push(item);
        trayObject.SetActive(shoppingTrayStack.Count > 0);
        animator.SetInteger("isCarryObjects", shoppingTrayStack.Count);
    }

    public void ClearShoppingTray()
    {
        float duration = 0f;
        while (shoppingTrayStack.Count > 0)
        {
            duration += Time.deltaTime;

            if (duration >= 0.2f)
            {
                MerchandiseItem item = shoppingTrayStack.Pop();
                item.transform.SetParent(null);
                ItemManager.Instance.ReturnItem(item.name, item);
                duration = 0f;
            }
        }

        trayObject.SetActive(shoppingTrayStack.Count > 0);
        animator.SetInteger("isCarryObjects", shoppingTrayStack.Count);
    }
}
