using System.Collections;
using UnityEngine;
using TMPro;

public class WorldTutorialFade : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    public TextMeshPro[] texts;
    public float fadeSpeed = 2f;

    Coroutine fadeRoutine;
    public void Start()
    {
        foreach (SpriteRenderer s in sprites)
        {
            Color c = s.color;
            c.a = 0f;
            s.color = c;
        }

        foreach (TextMeshPro t in texts)
        {
            Color c = t.color;
            c.a = 0f;
            t.color = c;
        }
    }
    public void FadeIn()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(Fade(1f));
    }

    public void FadeOut()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(Fade(0f));
    }

    IEnumerator Fade(float targetAlpha)
    {
        float currentAlpha = sprites[0].color.a;

        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

            foreach (SpriteRenderer s in sprites)
            {
                Color c = s.color;
                c.a = currentAlpha;
                s.color = c;
            }

            foreach (TextMeshPro t in texts)
            {
                Color c = t.color;
                c.a = currentAlpha;
                t.color = c;
            }

            yield return null;
        }
    }
}

