using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MyUtils;

public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    RectTransform rectTransform;
    CanvasGroup canvasGroup;

    [SerializeField]
    private Image imageBackground;
    [SerializeField]
    private float joystickFadeDuration;
    private Coroutine joystickFadeCoroutine;

    public Vector2 moveDir;

    void Awake()
    {
        if(rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        if(canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerDown");
        if (joystickFadeCoroutine != null)
            StopCoroutine(joystickFadeCoroutine);

        // 마우스 클릭 위치를 UI 좌표로 변환
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,                      // RectTransform 기준으로 좌표 변환
            eventData.position,                 // 클릭한 스크린 위치
            eventData.pressEventCamera,         // UI가 있는 카메라
            out localPoint                      // 변환된 좌표 저장
        );

        // UI 오브젝트 위치 이동
        imageBackground.rectTransform.anchoredPosition = localPoint;

        joystickFadeCoroutine = StartCoroutine(Util.SetFadeInOut(canvasGroup, true, joystickFadeDuration));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("PointerUp");
        if (joystickFadeCoroutine != null)
            StopCoroutine(joystickFadeCoroutine);

        joystickFadeCoroutine = StartCoroutine(Util.SetFadeInOut(canvasGroup, false, joystickFadeDuration));
    }

    public void OnDrag(PointerEventData eventData)
    {

    }
}
