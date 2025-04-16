using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class InteractedPad : MonoBehaviour
{
    [SerializeField]
    InteractedObjectBase interactedObj;

    [SerializeField]
    Collider _collider;

    [SerializeField]
    Material testMaterial;

    public float takeItemDelay;
    void Awake()
    {
        if (interactedObj == null)
            interactedObj = GetComponentInParent<InteractedObjectBase>();

        if (_collider == null)
            _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            //interactedObj.player = other.gameObject.GetComponent<PlayerController>();
            testMaterial.color = new Color(0f, 255f, 0f, 255f);
            interactedObj.TriggerEnter();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            if (other.gameObject.GetComponent<PlayerController>().isMoving) return;

            takeItemDelay += Time.deltaTime;

            if (takeItemDelay < 0.5f) return;

            interactedObj.TriggerStay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            testMaterial.color = new Color(0f, 255f, 255f, 255f);

            interactedObj.TriggerExit();
        }
    }
}
