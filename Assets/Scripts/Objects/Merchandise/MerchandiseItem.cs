using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum ItemType
{
    Burn = 0,
}

public class MerchandiseItem : MonoBehaviour
{
    public ItemType itemType;

    public void InitItem()
    {
        
    }

    public void JumpToPlayer(Transform endPos)
    {
        this.transform.DOJump(endPos.position, 1f, 1, 1f);
        this.transform.SetParent(endPos);
    }
}
