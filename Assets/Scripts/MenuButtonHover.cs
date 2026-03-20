using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;

    [Range(0f, 1f)]
    public float hoverAlpha = 0.6f;

    public float fadeSpeed = 8f; // higher = faster fade

    private float originalAlpha;
    private Coroutine fadeCoroutine;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalAlpha = buttonImage.color.a;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartFade(hoverAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartFade(originalAlpha);
    }

    void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeTo(targetAlpha));
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        while (Mathf.Abs(buttonImage.color.a - targetAlpha) > 0.01f)
        {
            Color c = buttonImage.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            buttonImage.color = c;
            yield return null;
        }

        // Snap exactly to target at the end
        Color finalColor = buttonImage.color;
        finalColor.a = targetAlpha;
        buttonImage.color = finalColor;
    }
}