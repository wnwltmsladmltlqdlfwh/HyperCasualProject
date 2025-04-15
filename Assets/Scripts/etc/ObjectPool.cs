using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> pool = new Queue<T>();
    private T _prefab;
    private Transform _parent;
    private string _name;

    public ObjectPool(T prefab, int initSize, Transform parent)
    {
        this._prefab = prefab;
        this._parent = parent;
        this._name = prefab.name;

        for (int i = 0; initSize > i; i++)
        {
            T _object = Object.Instantiate(prefab);
            _object.gameObject.name = _name;
            _object.gameObject.SetActive(false);

            if(_object.GetComponent<RectTransform>())
                _object.GetComponent<RectTransform>().SetParent(parent);
            else
                _object.transform.SetParent(parent);
            
            pool.Enqueue(_object);
        }
    }

    public T GetObject()
    {
        if(pool.Count > 0)
        {
            T _object = pool.Dequeue();
            _object.gameObject.SetActive(true);
            return _object;
        }
        else
        {
            T newObj = Object.Instantiate(_prefab);
            newObj.gameObject.name = _prefab.name;
            if(newObj.GetComponent<RectTransform>())
                newObj.GetComponent<RectTransform>().SetParent(_parent);
            else
                newObj.transform.SetParent(_parent);
            
            return newObj;
        }
    }

    public T FindObject(string name)
    {
        foreach (T obj in pool)
        {
            if (obj.gameObject.name == name && obj.gameObject.activeSelf == true)
            {
                return obj;
            }
        }
        return null;
    }

    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
