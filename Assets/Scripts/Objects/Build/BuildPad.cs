using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPad : InteractedObjectBase
{
    public InteractedObjectBase prefab;

    public int moneyCost;

    public override void TriggerEnter()
    {
        base.TriggerEnter();
        duration = 0f;
    }

    public override void TriggerStay()
    {
        base.TriggerStay();
        duration += Time.deltaTime;
        if (duration >= 5f)
        {
            var newInteractedObject = InteractedObjectManager.Instance.GetInteractedObject(prefab.name, this.transform);
            InteractedObjectManager.Instance.AddDineInTable(newInteractedObject.GetComponent<DineInTable>());
            gameObject.SetActive(false);
        }
    }

    public override void TriggerExit()
    {
        base.TriggerExit();
        duration = 0f;
    }
}
