using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    #region Cutscene Transition Settings
    [Header("Cutscene Transition Settings")]
    [SerializeField] private string endingSceneName = "Ending";
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    #endregion

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(FadeAndLoad());
        }
    }

    private IEnumerator FadeAndLoad()
    {
        float timer = 0f;

        // Fade to black
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Load next scene
        SceneManager.LoadScene(endingSceneName);
    }
}
        
