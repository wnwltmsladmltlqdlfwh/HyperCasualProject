using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI moneyTextPrefab;
    public CinemachineVirtualCamera eventCamera;

    private void Start()
    {

    }

    public void UpdateMoneyText(int money)
    {
        moneyTextPrefab.text = money.ToString();
    }

    public void CameraMoveToObject(Transform target)
    {
        float duration = 2f;

        eventCamera.Follow = target;
        eventCamera.LookAt = target;
        eventCamera.Priority = 2;

        while(duration > 0)
        {
            duration -= Time.deltaTime;
        }

        eventCamera.Priority = 0;
    }

    public IEnumerator MoveCameraToObject(Transform target)
    {
        Debug.LogError("MoveCameraToObject called with targetMoney: ");

        eventCamera.Follow = target;
        eventCamera.Priority = 2;

        yield return new WaitForSeconds(2f);

        eventCamera.Priority = 0;
        Debug.LogError("Camera moved to object is done");
    }
}
