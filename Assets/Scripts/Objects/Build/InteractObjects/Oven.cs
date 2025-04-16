using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class Oven : InteractedObjectBase
{
    [SerializeField]
    private Transform burnObject;
    [SerializeField]
    private Transform burnBoxTransform;

    private Vector3 setStartPosition;

    [SerializeField]
    private Stack<MerchandiseItem> burnStack = new Stack<MerchandiseItem>();

    [SerializeField]
    private float setBakedAnimTime;
    private Tween animTween;
    private float bakedTime;
    public int capacity;
    private int reservedItemCount = 0;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        capacity = 10;
        setStartPosition = new Vector3(0f, 0.6f, 0f);
        burnObject.transform.localPosition = setStartPosition;

        animTween = burnObject.DOLocalMoveZ(-2.0f, setBakedAnimTime)
                    .SetAutoKill(false)
                    .Pause()
                    .OnComplete(() => BakedNewBurn());

        StartCoroutine(OvenInBurnMove());
    }

    IEnumerator OvenInBurnMove()
    {
        while (true)
        {
            yield return new WaitUntil(() => burnStack.Count < capacity);

            animTween.Restart();

            yield return new WaitUntil(() => !animTween.IsPlaying());

            animTween.Pause();
        }
    }

    private void Update()
    {
        /*
        if (burnStack.Count < capacity)
        {
            bakedTime += Time.deltaTime;

            if (bakedTime >= setBakedAnimTime)
            {
                // 빵 스폰
                var newBurn = ItemManager.Instance.GetItem("Burn");
                newBurn.InitItem();
                burnStack.Push(newBurn);
                newBurn.transform.position = burnBoxTransform.position + new Vector3(0f, 2f, 0f);
                bakedTime = 0f;
            }
        }
        */
    }

    private void BakedNewBurn()
    {
        var newBurn = ItemManager.Instance.GetItem("Burn");
        newBurn.InitItem();
        burnStack.Push(newBurn);
        newBurn.transform.position = burnBoxTransform.position + new Vector3(0f, 2f, 0f);
    }

    public override void TriggerEnter()
    {
        base.TriggerEnter();

        duration = 0f;
    }

    public override void TriggerStay()
    {
        base.TriggerStay();

        if (GameManager.Instance.player.objectStack.Count + reservedItemCount >= GameManager.Instance.player.objectCapacity)
            return;

        duration += Time.deltaTime;
        if (duration >= 0.2f)
        {
            if (burnStack.Count <= 0) return;

            var takedBurn = burnStack.Pop();
            reservedItemCount++;

            takedBurn.transform.DOJump(GameManager.Instance.player.overTray.position, 1f, 1, 1f).OnComplete(() =>
            {
                GameManager.Instance.player.AddObjectList(takedBurn);
                reservedItemCount--;
            });

            duration = 0f;
        }
    }

    public override void TriggerExit()
    {
        base.TriggerExit();

        duration = 0f;
    }
}
