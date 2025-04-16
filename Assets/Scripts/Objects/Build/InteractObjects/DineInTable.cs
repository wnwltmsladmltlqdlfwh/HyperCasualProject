using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DineInTable : InteractedObjectBase
{
    public enum DineInTableState
    {
        Empty,
        Reserved,
        Occupied,
        NeedCleaning,
    }

    private DineInTableState _dineInTableState;
    public DineInTableState TableStates
    {
        get => _dineInTableState;
        set
        {
            if(_dineInTableState == value)
                return;
            var oldState = _dineInTableState;
            _dineInTableState = value;
            if(oldState == DineInTableState.NeedCleaning && _dineInTableState == DineInTableState.Empty)
                GetMoney();

            OnTableSettingState();
        }
    }

    public GameObject tableSettingPrefab;
    public GameObject trashPrefab;

    public int customerNeedItemCapacity;

    public Transform seatChairTransform;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private MoneySpawn moneySpawnPoint;

    public void InitTable()
    {
        _dineInTableState = DineInTableState.Empty;
        tableSettingPrefab.SetActive(false);
        trashPrefab.SetActive(false);
    }

    public override void TriggerEnter()
    {
        base.TriggerEnter();
    }

    public override void TriggerStay()
    {
        base.TriggerStay();
        if (_dineInTableState != DineInTableState.NeedCleaning)
            return;

        duration += Time.deltaTime;
        if (duration >= 0.5f)
        {
            TableStates = DineInTableState.Empty;
            duration = 0f;
        }
    }

    public override void TriggerExit()
    {
        base.TriggerExit();
    }

    private void OnTableSettingState()
    {
        if(tableSettingPrefab == null || trashPrefab == null)
        {
            Debug.LogError("Table setting or trash prefab is null, check");
            return;
        }

        tableSettingPrefab.SetActive(_dineInTableState == DineInTableState.Occupied);
        trashPrefab.SetActive(_dineInTableState == DineInTableState.NeedCleaning);

        if(_dineInTableState == DineInTableState.NeedCleaning)
        {
            _animator.SetTrigger("isDirty");
        }
        else if(_dineInTableState == DineInTableState.Empty)
        {
            _animator.SetTrigger("isClean");
        }
    }

    private void GetMoney()
    {
        for(int i = 0; i < customerNeedItemCapacity + 1; i++)
        {
            var newMoney = GameManager.Instance.GetMoney();
            moneySpawnPoint.AddMoneyStack(newMoney);
        }

        customerNeedItemCapacity = 0;
    }
}
