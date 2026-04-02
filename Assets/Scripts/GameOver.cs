using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private string endingSceneName = "EndingCutScene"; // Name of the scene with your cutscene

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Load the ending cutscene scene
            SceneManager.LoadScene(endingSceneName);
        }
    }
}