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
    public GameObject boxingPrefab;

    [SerializeField]
    private MoneySpawn moneySpawnPoint;

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
                                            if(!customer.eatInShop)
                                                GetMoney(customer);

                                            customer.ClearShoppingTray();
                                            UpdatePayCustomersQueue();
                                        });
    }

    public override void TriggerExit()
    {
        base.TriggerExit();
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

    public void GetMoney(Customer customer)
    {
        for(int i = 0; i < customer.needItemCapacity; i++)
        {
            var newMoney = GameManager.Instance.GetMoney();
            moneySpawnPoint.AddMoneyStack(newMoney);
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
