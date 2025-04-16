using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractedObjectManager : Singleton<InteractedObjectManager>
{
    public Oven ovenPrefab;
    public CashierCounter cashierCounter;

    public DisplayTable displayTablePrefab;
    public Dictionary<ItemType, DisplayTable> displayTableDict = new Dictionary<ItemType, DisplayTable>();

    public DineInTable dineInTablePrefab;
    [SerializeField]
    private List<DineInTable> dineInTableList = new List<DineInTable>();

    private Queue<BuildPad> buildPadQueue = new Queue<BuildPad>();

    public List<BuildPad> buildPadList = new List<BuildPad>();

    public Dictionary<string, ObjectPool<InteractedObjectBase>> interactedObjectPoolDict = new Dictionary<string, ObjectPool<InteractedObjectBase>>();

    void Start()
    {
        InitObejcts();
    }

    private void InitObejcts()
    {
        CreatePool(dineInTablePrefab, 3);
        foreach(var buildPad in buildPadList)
        {
            buildPadQueue.Enqueue(buildPad);
            Debug.Log("BuildPad: " + buildPad.name + " added to queue.");
        }
    }

    public BuildPad GetBuildPad()
    {
        if (buildPadQueue.Count == 0)
            return null;

        return buildPadQueue.Dequeue();
    }

    public void AddDineInTable(DineInTable dineInTable)
    {
        if (dineInTableList.Contains(dineInTable))
            return;

        dineInTableList.Add(dineInTable);
    }

    public DineInTable GetEmptyDineInTable()
    {
        if (dineInTableList.Count == 0)
            return null;

        for (int i = 0; i < dineInTableList.Count; i++)
        {
            if (dineInTableList[i].TableStates == DineInTable.DineInTableState.Empty)
            {
                return dineInTableList[i];
            }
        }

        return null;
    }

    public void CreatePool(InteractedObjectBase interactedObjPrefab, int initSize)
    {
        Transform parentTransform = new GameObject(interactedObjPrefab.name + "_pool").transform;
        parentTransform.SetParent(this.transform);
        var createdPool = new ObjectPool<InteractedObjectBase>(interactedObjPrefab, initSize, parentTransform);
        interactedObjectPoolDict[interactedObjPrefab.name] = createdPool;
    }

    public InteractedObjectBase GetInteractedObject(string prefabName, Transform buildPosition)
    {
        if (interactedObjectPoolDict.ContainsKey(prefabName))
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
        if (interactedObjectPoolDict.ContainsKey(prefabName))
        {
            interactedObjectPoolDict[prefabName].ReturnObject(interactedObj);
        }
    }
}
