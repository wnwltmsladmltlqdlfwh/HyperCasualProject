using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildPad : InteractedObjectBase
{
    public InteractedObjectBase buildPrefab;

    [SerializeField]
    private TextMeshProUGUI buildCostText;

    public int moneyCost;

    private void Update()
    {
        buildCostText.text = moneyCost.ToString();
    }

    public override void TriggerEnter()
    {
        base.TriggerEnter();
        duration = 0f;
    }

    public override void TriggerStay()
    {
        base.TriggerStay();
        duration += Time.deltaTime;
        if (duration >= 0.5f)
        {
            if (GameManager.Instance.Money <= 0)
                return;
            else if (moneyCost <= 0)
            {
                var newInteractedObject = InteractedObjectManager.Instance.GetInteractedObject(buildPrefab.name, this.transform);
                InteractedObjectManager.Instance.AddDineInTable(newInteractedObject.GetComponent<DineInTable>());
                gameObject.SetActive(false);
                return;
            }

            moneyCost -= 10;
            GameManager.Instance.Money -= 10;
            var showCoin = GameManager.Instance.GetMoney();
            showCoin.transform.position = GameManager.Instance.player.transform.position;
            showCoin.transform.DOJump(this.transform.position, 0.5f, 1, 0.5f)
                .OnComplete(() =>
                {
                    GameManager.Instance.ReturnMoney(showCoin);
                });
            duration = 0.45f;
        }
    }

    public override void TriggerExit()
    {
        base.TriggerExit();
        duration = 0f;
    }
}
