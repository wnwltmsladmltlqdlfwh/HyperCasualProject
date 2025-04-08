using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace MyUtils
{
    public static class Util
    {
        #region UI 관련
        // FadeIn, Out 효과를 관리하는 코루틴, isFade가 true일땐 FadeIn, false 일땐 FadeOut
        public static IEnumerator SetFadeInOut(CanvasGroup canvasGroup, bool isFade, float duration)
        {
            float elapsed = 0f;

            float startAlpha = canvasGroup.alpha;
            float endAlpha = isFade ? 1f : 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                canvasGroup.alpha = alpha;
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }
        #endregion
    }
}
