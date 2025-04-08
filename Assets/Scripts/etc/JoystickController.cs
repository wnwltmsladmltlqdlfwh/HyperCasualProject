using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static MyUtils.Util;
using JetBrains.Annotations;

public class JoystickController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform background;
    [SerializeField]
    private RectTransform handle;
    [SerializeField]
    private float handleRange = 100f;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private float fadeDuration = 0.1f;

    private Vector2 input = Vector2.zero;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        background.position = eventData.position;
        handle.anchoredPosition = Vector2.zero;
        if(fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(SetFadeInOut(canvasGroup, true, fadeDuration));
        
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 position))
        {
            // 입력값을 -1 ~ 1 범위로 정규화
            input = position / handleRange;
            
            // 입력값의 크기가 1을 넘지 않도록 제한
            if (input.magnitude > 1)
                input = input.normalized;

            // 핸들 위치 업데이트
            handle.anchoredPosition = input * handleRange;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 입력값 초기화
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        // 페이드 아웃 효과
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(SetFadeInOut(canvasGroup, false, fadeDuration));
    }

    public Vector3 GetMoveDirection()
    {
        return new Vector3(input.x, 0f, input.y);
    }
}
