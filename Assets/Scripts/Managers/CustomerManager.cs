using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : Singleton<CustomerManager>
{
    public Customer customerPrefab;

    public ObjectPool<Customer> customerPool;

    public bool loadComplete = false;

    [SerializeField]
    private Transform customerSpawnPos;
    public Transform customerEnterPos;
    public Dictionary<ItemType, DisplayTable> displayTableDict = new Dictionary<ItemType, DisplayTable>();

    private int customerCurrentCount;
    [SerializeField]
    private int customerCapacity;

    public float duration;
    public bool isOpenShop = false;

    void Start()
    {
        LoadItemPrefabs();
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
        newCustomer.transform.position = customerSpawnPos.position;
        newCustomer.InitCustomer();
        customerCurrentCount++;
    }

    public void LoadItemPrefabs()
    {
        CreatePool(customerPrefab, 3);

        loadComplete = true;
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

    public void ReturnNPC(Customer item)
    {
        customerPool.ReturnObject(item);
        customerCurrentCount--;
    }
}
