using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    void Start()
    {
        // Start the fade in from black when the scene loads
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        // Call this method to initiate a scene change
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            yield return null;
        }
        // Deactivate the image after fading in to allow interaction
        fadeImage.gameObject.SetActive(false);
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        // Reactivate the image to begin the fade out
        fadeImage.gameObject.SetActive(true);
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
        // When the new scene loads, the Start() method will call FadeIn() automatically
    }
}
