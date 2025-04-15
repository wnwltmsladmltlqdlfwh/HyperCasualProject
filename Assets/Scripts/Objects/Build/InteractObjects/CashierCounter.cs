using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CashierCounter : InteractedObjectBase
{
    public Queue<Customer> payCustomersQueue = new Queue<Customer>();

    public Queue<Customer> dineInCustomersQueue = new Queue<Customer>();

    public Text testForCheckCustomerCount;

    public GameObject boxingPrefab;

    private void Update()
    {
        if (testForCheckCustomerCount != null)
        {
            testForCheckCustomerCount.text = "Queue Count : " + payCustomersQueue.Count;
        }
    }

    public override void TriggerEnter()
    {
        base.TriggerEnter();
        duration = 0f;
    }

    public override void TriggerStay()
    {
        base.TriggerStay();

        if (payCustomersQueue.Count <= 0)
            return;

        if (duration < 1f)
            duration += Time.deltaTime;
        else if (duration >= 1f)
        {
            Customer dequeueCustomer = payCustomersQueue.Dequeue();

            _ = StartCoroutine(PackagingBox(dequeueCustomer));

            duration = 0f;
        }
    }

    private IEnumerator PackagingBox(Customer customer)
    {
        if (boxingPrefab == null)
        {
            Debug.LogError("Boxing prefab is null, check");
            yield break;
        }

        boxingPrefab.SetActive(true);
        boxingPrefab.GetComponent<Animator>().SetTrigger("Packaging");
        yield return new WaitForSeconds(1f);
        boxingPrefab.SetActive(false);
        customer.transform.DOJump(customer.transform.position, 0.2f, 3, 0.5f)
                                        .OnComplete(() =>
                                        {
                                            customer.ClearShoppingTray();
                                            UpdatePayCustomersQueue();
                                        });
    }

    public override void TriggerExit()
    {
        base.TriggerExit();
    }

    private int ReturnItemPrice(ItemType itemType, int itemCount)
    {
        // 추후 재화 아이템을 제작 후, 아이템을 풀링 해주는 함수만들기,
        int price;

        // 가격은 ItemType에 따라 다르게 설정
        switch (itemType)
        {
            case ItemType.Burn:
                price = 100 * itemCount;
                break;

            default:
                price = 0;
                Debug.LogError("Customer's ItemType is null, check");
                break;
        }
        return price;
    }

    public void UpdatePayCustomersQueue()
    {
        if (payCustomersQueue.Count <= 0)
            return;

        int index = 0;

        foreach (var customer in payCustomersQueue)
        {
            Vector3 targetPos = transform.position + new Vector3(0f, 0f, 1f + (index * 1.5f));
            customer.navMeshAgent.SetDestination(targetPos);
            index++;
        }
    }

    public void UpdateDineInQueue()
    {
        if (dineInCustomersQueue.Count <= 0)
            return;

        int index = 0;

        foreach (var customer in dineInCustomersQueue)
        {
            Vector3 targetPos = transform.position + new Vector3(2f + (index * 1.5f), 0f, 0f);
            customer.navMeshAgent.SetDestination(targetPos);
            index++;
        }
    }
}
