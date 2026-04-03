using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroCutscene : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    #region Fade Settings
    public float fadeDuration = 1f;
    public Image fadeImage;
    #endregion

    public string nextScene;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Wait for video to finish playing or skip
        if (Input.GetKeyDown(KeyCode.E))
        {
            videoPlayer.Stop();
            videoPlayer.targetTexture.Release();
            StartCoroutine(FadeOutAndLoad(nextScene));
        }

        if (!videoPlayer.isPlaying)
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
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
    }
}