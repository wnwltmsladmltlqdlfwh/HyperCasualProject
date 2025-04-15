using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Lumin;

public enum ItemType
{
    Burn = 0,
}

public class MerchandiseItem : MonoBehaviour
{
    [SerializeField]
    private Transform itemPrefab;
    private Rigidbody _rigidbody;
    private BoxCollider _collider;

    public ItemType itemType;
    public int price;
    public float topY;
    
    public void InitItem()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        if (_collider == null)
            _collider = GetComponent<BoxCollider>();

        if(_rigidbody.isKinematic == true)
            TurnOnPhysics(true);

        // 프리팹을 가져와서 collider 사이즈에 맞추기
        Renderer[] renderers = itemPrefab.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("Renderer is null");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        // 월드 좌표 → 로컬 좌표 변환
        _collider.center = transform.InverseTransformPoint(bounds.center);
        _collider.size = bounds.size;

        topY = bounds.max.y;
    }


    /// <summary>
    /// rigidbody의 물리 충돌 사용 여부
    /// </summary>
    /// <param name="isOn">true = 사용, false = 사용안함</param>
    public void TurnOnPhysics(bool isOn)
    {
        if (_rigidbody == null || _collider == null)
        {
            Debug.LogError($"{this.gameObject.name} : Need Check Rigidbody or Collider Component");
            return;
        }

        if (isOn == false)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }
        else
        {
            _rigidbody.isKinematic = false;
        }

    }
}
