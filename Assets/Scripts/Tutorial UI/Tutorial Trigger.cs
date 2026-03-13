using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    
    public WorldTutorialFade tutorial; // Reference to the tutorial popup

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tutorial.FadeIn();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tutorial.FadeOut();
        }
    }
}