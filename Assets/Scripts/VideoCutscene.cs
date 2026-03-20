using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float fadeDuration = 1f;
    public Image fadeImage;
    public string nextScene;

    private void Update()
    {
        // Wait for video to finish playing or skip
        if (Input.GetKeyDown(KeyCode.E))
        {
            videoPlayer.Stop();
            StartCoroutine(FadeOutAndLoad(nextScene));
            //Debug.Log("Test");
        }

        if (!videoPlayer.isPlaying)
        {
            SceneManager.LoadScene(nextScene);
            //Debug.Log("Video Ended");
        }
    }

    IEnumerator PlayVid()
    {
        //yield return null;

        videoPlayer.Play();

        // Wait for video to finish playing or skip
        while (videoPlayer.isPlaying)
        {
            Debug.Log("Video Playing");

            if (Input.GetKeyDown(KeyCode.E))
            {
                videoPlayer.Stop();
                //yield return StartCoroutine(FadeToBlack());
                break;
            }

            yield return null;
        }
        SceneManager.LoadScene(nextScene);
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