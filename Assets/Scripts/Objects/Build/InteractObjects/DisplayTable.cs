using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class DisplayTable : InteractedObjectBase
{
    [SerializeField]
    private Transform setDisplayItems;

    private Stack<MerchandiseItem> displayItems = new Stack<MerchandiseItem>();

    public ItemType itemType;

    public Queue<Customer> customersQueue = new Queue<Customer>();

    public int capacity;

    void Start()
    {
        InitDisplayTable();
    }

    public void InitDisplayTable()
    {
        CustomerManager.Instance.displayTableDict.Add(itemType, this);
    }

    public override void TriggerEnter()
    {
        base.TriggerEnter();

        duration = 0f;
    }

    public override void TriggerStay()
    {
        base.TriggerStay();

        if (displayItems.Count >= capacity)
            return;

        duration += Time.deltaTime;
        if (duration >= 0.2f)
        {
            var displayBurn = player.FindObjectInList(itemType);

            if (displayBurn == null)
                return;

            displayItems.Push(displayBurn);
            displayBurn.transform.SetParent(setDisplayItems);

            int index = displayItems.Count - 1;
            Vector3 targetPos = new Vector3(0f, index * 0.3f, 0f);

            displayBurn.transform.DOJump(setDisplayItems.position, 1f, 1, 1f)
                                    .OnComplete(() =>
                                    {
                                        displayBurn.transform.localPosition = targetPos;
                                        displayBurn.transform.localRotation = Quaternion.identity;
                                    }
                                    );

            duration = 0f;
        }
    }

    private void SetOnDisplayTable(MerchandiseItem addedItem)
    {
        for (int i = 0; i < setDisplayItems.childCount; i++)
        {
            if (setDisplayItems.GetChild(i) == addedItem)
            {
                //displayBurn.transform.localPosition = new Vector3(0f, index * 0.3f, 0f);
                Debug.Log(addedItem.transform.localPosition.y);
                break;
            }
        }

        addedItem.transform.localRotation = Quaternion.identity;
    }

    public override void TriggerExit()
    {
        base.TriggerExit();
    }

    public void UpdateCustomersQueue()
    {
        int index = 0;

        foreach(var customer in customersQueue)
        {
            Vector3 targetPos = transform.position + new Vector3(0f, 0f, -1f - (index * 1f));
            customer.navMeshAgent.SetDestination(targetPos);
            index++;
        }
    }
}
