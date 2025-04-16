using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    public List<MerchandiseItem> itemBases;
    public Dictionary<string, Transform> parentTransformDict = new Dictionary<string, Transform>();
    private Dictionary<string, ObjectPool<MerchandiseItem>> itemPoolDict = new Dictionary<string, ObjectPool<MerchandiseItem>>();

    public bool loadComplete = false;

    void Start()
    {
        LoadItemPrefabs();
    }

    public void LoadItemPrefabs()
    {
        var Items = Resources.LoadAll<MerchandiseItem>("Prefabs/Items");
        foreach (var item in Items)
        {
            CreatePool(item, 3);
        }

        loadComplete = true;
    }

    public void CreatePool(MerchandiseItem itemPrefab, int initSize)
    {
        Transform parentTransform = new GameObject(itemPrefab.itemType.ToString() + "_pool").transform;
        parentTransform.SetParent(this.transform);
        parentTransformDict[itemPrefab.itemType.ToString()] = parentTransform;
        itemPoolDict[itemPrefab.itemType.ToString()] = new ObjectPool<MerchandiseItem>(itemPrefab, initSize, parentTransform);
    }

    public MerchandiseItem GetItem(string prefabName)
    {
        if (itemPoolDict.ContainsKey(prefabName))
        {
            return itemPoolDict[prefabName].GetObject();
        }

        return null;
    }

    public void ReturnItem(string prefabName, MerchandiseItem item)
    {
        if (itemPoolDict.ContainsKey(prefabName))
        {
            itemPoolDict[prefabName].ReturnObject(item);
        }
        if(parentTransformDict.ContainsKey(prefabName))
        {
            item.transform.SetParent(parentTransformDict[prefabName]);
        }
    }
}