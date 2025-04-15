using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using System.Linq;

public class DisplayTable : InteractedObjectBase
{
    [SerializeField]
    private Transform setDisplayItems;

    public Stack<MerchandiseItem> displayItems = new Stack<MerchandiseItem>();

    public ItemType itemType;

    public Queue<Customer> customersQueue = new Queue<Customer>();

    public int capacity;

    void Start()
    {
        InitDisplayTable();
    }

    public void InitDisplayTable()
    {
        InteractedObjectManager.Instance.displayTableDict.Add(itemType, this);
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

            int index = displayItems.Count;

            displayItems.Push(displayBurn);
            displayBurn.transform.SetParent(setDisplayItems);

            int xPos = (index % 3) - 1;
            int yPos = index / 6;
            int zPos = index / 3;

            Vector3 targetPos = (zPos % 2 == 0)
                ? new Vector3(xPos, yPos * 0.3f, 0.4f) : new Vector3(xPos, yPos * 0.3f, -0.4f);

            displayBurn.transform.DOLocalJump(targetPos, 1f, 1, 1f)
                                    .OnComplete(() =>
                                    {
                                        displayBurn.transform.localPosition = targetPos;
                                        displayBurn.transform.localRotation = Quaternion.identity;
                                    }
                                    );

            duration = 0f;
        }
    }

    public override void TriggerExit()
    {
        base.TriggerExit();
    }

    public void GiveItemToCustomer(Customer customer, Transform targetTransform)
    {
        if (displayItems.Count == 0)
        {
            Debug.Log("DisplayTable : No Item in DisplayTable");
            return;
        }

        if (customer.shoppingTrayStack.Count >= customer.needItemCapacity)
        {
            return;
        }

        MerchandiseItem item = displayItems.Pop();

        item.transform.SetParent(customer.trayObject.transform);

        customer.ReceiveItem(item);
        int currentItemCount = customer.shoppingTrayStack.Count - 1;
        item.transform.DOLocalJump(targetTransform.position, 1f, 1, 1f)
                      .OnComplete(() =>
                      {
                            item.transform.localRotation = Quaternion.identity;
                            item.transform.localPosition = new Vector3(0f, 0.3f + (0.6f * currentItemCount), 0f);
                      });
    }

    public void UpdateCustomersQueue()
    {
        if (customersQueue.Count <= 0)
            return;

        int index = 0;

        foreach (var customer in customersQueue)
        {
            Vector3 targetPos = transform.position + new Vector3(0f, 0f, -1f - (index * 1.5f));
            customer.navMeshAgent.SetDestination(targetPos);
            index++;
        }
    }
}
