using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerController player;

    public Money moneyPrefab;
    public ObjectPool<Money> moneyPool;
    Transform parentTransform;

    [SerializeField]
    private int _money;
    public int Money
    {
        get { return _money; }
        set
        {
            _money = value;
            if(_money == 300)
                _= StartCoroutine(UIManager.Instance.MoveCameraToObject(InteractedObjectManager.Instance.GetBuildPad().transform));
                //UIManager.Instance.CameraMoveToObject(InteractedObjectManager.Instance.GetBuildPad().transform);

            UIManager.Instance.UpdateMoneyText(_money);
        }
    }

    private void Start()
    {
        InitGameManager();
    }

    public void InitGameManager()
    {
        Money = 0;
        CreatePool(moneyPrefab, 10);
    }

    public void CreatePool(Money moneyPrfab, int initSize)
    {
        if (parentTransform == null)
            parentTransform = new GameObject("MoneyPrefab_pool").transform;

        parentTransform.SetParent(this.transform);
        moneyPool = new ObjectPool<Money>(moneyPrfab, initSize, parentTransform);
    }

    public Money GetMoney()
    {
        return moneyPool.GetObject();
    }

    public void ReturnMoney(Money money)
    {
        moneyPool.ReturnObject(money);
        money.transform.SetParent(parentTransform);
    }
}
