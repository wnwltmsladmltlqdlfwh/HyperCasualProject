using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoneySpawn : InteractedObjectBase
{
    public Stack<Money> moneyStack = new Stack<Money>();
    private bool isTakingMoney;

    public override void TriggerEnter()
    {
        base.TriggerEnter();
        duration = 0f;
        isTakingMoney = false;
    }

    public override void TriggerStay()
    {
        base.TriggerStay();

        if (moneyStack.Count <= 0)
            return;

        duration += Time.deltaTime;

        if (duration >= 0.5f && !isTakingMoney)
        {
            isTakingMoney = true;
        }
        else if (duration >= 0.1f && isTakingMoney)
        {
            Money money = moneyStack.Pop();
            money.transform.SetParent(null);
            money.transform.DOJump(GameManager.Instance.player.transform.position, 1f, 1, 0.5f).SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    GameManager.Instance.ReturnMoney(money);
                    GameManager.Instance.Money += 50;
                    duration = 0f;
                });
        }
    }

    public override void TriggerExit()
    {
        base.TriggerExit();
        duration = 0f;
        isTakingMoney = false;
    }

    public void AddMoneyStack(Money money)
    {
        if (money == null)
            return;

        int index = moneyStack.Count;

        moneyStack.Push(money);
        money.transform.SetParent(this.transform);
        money.transform.localPosition = new Vector3(0f, 2f, 0f);
        money.transform.localRotation = Quaternion.identity;

        int xPos = (index % 3) - 1;
        int yPos = index / 9;
        int zPos = (index / 3) % 3;

        Vector3 targetPos = new Vector3(
            xPos * 0.7f,            // X position
            yPos * 0.3f,            // Y position
            0.5f - zPos * 0.5f);    // Z position

        money.transform.DOLocalJump(targetPos, 1f, 1, 0.3f).SetEase(Ease.InOutBack)
                                .OnComplete(() =>
                                {
                                    money.transform.localPosition = targetPos;
                                    money.transform.localRotation = Quaternion.identity;
                                }
                                );
    }
}
