using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class Oven : MonoBehaviour
{
    [SerializeField]
    private Transform burnObject;
    [SerializeField]
    private Transform burnBoxTransform;

    private Vector3 setStartPosition;

    [SerializeField]
    private Stack<MerchandiseItem> burnStack = new Stack<MerchandiseItem>();

    [SerializeField]
    private float setBakedBurnTime;
    private float bakedTime;
    public int capacity;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        capacity = 10;
        setStartPosition = new Vector3(0f, 0.6f, 0f);
        burnObject.transform.localPosition = setStartPosition;

        //burnObject.DOLocalMoveZ(-2.0f, 5f).SetLoops(-1, LoopType.Restart);
        StartCoroutine(OvenInBurnMove());
    }

    IEnumerator OvenInBurnMove()
    {
        while(true)
        {
            yield return new WaitUntil(() => burnStack.Count < capacity);

            burnObject.DOLocalMoveZ(-2.0f, setBakedBurnTime);

            yield return new WaitUntil(() => burnObject.transform.localPosition.z == -2.0f);

            burnObject.transform.localPosition = setStartPosition;
        }
    }

    private void Update()
    {
        if (burnStack.Count < capacity)
        {
            bakedTime += Time.deltaTime;

            if (bakedTime >= setBakedBurnTime)
            {
                // 빵 스폰
                var newBurn = ItemManager.Instance.GetItem("Burn");
                burnStack.Push(newBurn);
                newBurn.transform.position = burnBoxTransform.position + new Vector3(0f, 2f, 0f);
                bakedTime = 0f;
            }
        }
    }

    public void TakeItem(PlayerController player)
    {
        if(burnStack.Count <= 0) return;

        var takedBurn = burnStack.Pop();
        takedBurn.JumpToPlayer(player.overTray);
        player.PushObjectStack(takedBurn);
    }
}
