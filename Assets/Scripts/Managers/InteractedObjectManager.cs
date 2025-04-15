using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractedObjectManager : Singleton<InteractedObjectManager>
{
    public Oven ovenPrefab;
    public CashierCounter cashierCounter;

    public DisplayTable displayTablePrefab;
    public Dictionary<ItemType, DisplayTable> displayTableDict = new Dictionary<ItemType, DisplayTable>();

    [SerializeField]
    DineInTable dineInTablePrefab;
    private List<DineInTable> dineInTableList = new List<DineInTable>();

    public Dictionary<string, ObjectPool<InteractedObjectBase>> interactedObjectPoolDict = new Dictionary<string, ObjectPool<InteractedObjectBase>>();

    void Start()
    {
        InitObejcts();
    }

    private void InitObejcts()
    {
        CreatePool(dineInTablePrefab, 3);
    }

    public void CreatePool(InteractedObjectBase interactedObjPrefab, int initSize)
    {
        Transform parentTransform = new GameObject("Customer_pool").transform;
        parentTransform.SetParent(this.transform);
        var createdPool = new ObjectPool<InteractedObjectBase>(interactedObjPrefab, initSize, parentTransform);
        interactedObjectPoolDict[interactedObjPrefab.name] = createdPool;
    }

    public InteractedObjectBase GetInteractedObject(string prefabName, Transform buildPosition)
    {
        if(interactedObjectPoolDict.ContainsKey(prefabName))
        {
            InteractedObjectBase interactedObj = interactedObjectPoolDict[prefabName].GetObject();
            interactedObj.transform.position = buildPosition.position;
            return interactedObj;
        }

        return null;
    }

    public InteractedObjectBase FindInteractedObject(string name)
    {
        foreach (var pool in interactedObjectPoolDict.Values)
        {
            var obj = pool.FindObject(name);
            if (obj != null)
            {
                return obj;
            }
        }
        Debug.LogError("Object not found in any pool: " + name);
        return null;
    }

    public void ReturnInteractedObject(string prefabName, InteractedObjectBase interactedObj)
    {
        if(interactedObjectPoolDict.ContainsKey(prefabName))
        {
            interactedObjectPoolDict[prefabName].ReturnObject(interactedObj);
        }
    }
}
