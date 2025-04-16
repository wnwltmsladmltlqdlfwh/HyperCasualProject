using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractedObjectBase : MonoBehaviour
{
    public float duration;

    public virtual void TriggerEnter() { }

    public virtual void TriggerStay()
    {
        if (GameManager.Instance.player == null)
            return;
        else if(GameManager.Instance.player.isMoving)
            return;
    }

    public virtual void TriggerExit() { }
}
