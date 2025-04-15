using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : Singleton<CustomerManager>
{
    public Customer customerPrefab;

    public ObjectPool<Customer> customerPool;

    [SerializeField]
    private Transform customerSpawnPos;
    public Transform customerEnterPos;
    public List<Customer> forCheckCustomerList = new List<Customer>();

    private int customerCurrentCount;
    [SerializeField]
    private int customerCapacity;

    public float duration;
    public bool isOpenShop = false;

    void Start()
    {
        CreatePool(customerPrefab, 5);
    }

    void Update()
    {
        if (customerCurrentCount >= customerCapacity || isOpenShop == false) return;

        duration += Time.deltaTime;

        if (duration > 3f)
        {
            SpawnCustomer();
            duration = 0f;
        }
    }

    private void SpawnCustomer()
    {
        var newCustomer = customerPool.GetObject();
        forCheckCustomerList.Add(newCustomer);
        
        for(int i = 0; i < forCheckCustomerList.Count; i++)
        {
            if (forCheckCustomerList[i] == newCustomer)
            {
                newCustomer.gameObject.name = "Customer_" + i.ToString();
                break;
            }
        }

        newCustomer.transform.position = customerSpawnPos.position;
        newCustomer.InitCustomer();
        customerCurrentCount++;
    }

    public void CreatePool(Customer customerPrefab, int initSize)
    {
        Transform parentTransform = new GameObject("Customer_pool").transform;
        parentTransform.SetParent(this.transform);
        customerPool = new ObjectPool<Customer>(customerPrefab, initSize, parentTransform);
    }

    public Customer GetNPC()
    {
        return customerPool.GetObject();
    }

    public void ReturnNPC(Customer customer)
    {
        customerPool.ReturnObject(customer);
        customerCurrentCount--;
    }
}
