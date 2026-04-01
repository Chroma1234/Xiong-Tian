using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndingCutsceneController : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;

    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "MainMenu"; // Set to your main menu scene

    private void Start()
    {
        // Start the video
        videoPlayer.Play();

        // Subscribe to the video end event
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Load the main menu scene when video finishes
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        videoPlayer.loopPointReached -= OnVideoEnd;
    }
}