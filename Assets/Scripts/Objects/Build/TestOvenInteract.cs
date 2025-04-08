using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class TestOvenInteract : MonoBehaviour
{
    [SerializeField]
    Oven oven;
    [SerializeField]
    Collider _collider;

    [SerializeField]
    Material testMaterial;

    [SerializeField]
    PlayerController player;

    public float takeItemDelay;
    public float duration;

    void Awake()
    {
        if (oven == null)
            oven = GetComponentInParent<Oven>();

        if (_collider == null)
            _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            duration = 0f;
            player = other.gameObject.GetComponent<PlayerController>();
            testMaterial.color = new Color(0f, 255f, 0f, 255f);
            Debug.Log($"상호작용 시작 : duration" + duration);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (player.objectStack.Count >= player.objectCapacity)
            return;
        
        takeItemDelay += Time.deltaTime;
        if(takeItemDelay < 1f) return;

        duration += Time.deltaTime;
        if (duration >= 0.2f)
        {
            duration = 0f;
            oven.TakeItem(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            testMaterial.color = new Color(0f, 255f, 255f, 255f);
            player = null;
            duration = 0f;
        }
    }
}
