using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    private BoxCollider2D tutorialCollider;

    [SerializeField] private CanvasGroup[] tutorialPages;
    [SerializeField] private VideoClip[] videoClips;

    private int currentPage = 0;

    private bool tutorialSeen = false;
    private bool tutorialOpen = false;

    [SerializeField] private GameManager manager;

    [SerializeField] private VideoPlayer player;

    private void Awake()
    {
        tutorialCollider = GetComponent<BoxCollider2D>();

        // Hide all tutorial pages initially
        foreach (CanvasGroup page in tutorialPages)
        {
            page.alpha = 0f;
            page.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!tutorialOpen) return;

        // ESC only works on the last page
        if (Input.GetKeyDown(KeyCode.E) && currentPage == tutorialPages.Length - 1)
        {
            ClosePrompt();
        }

        else if (Input.GetKeyDown(KeyCode.E))
        {
            NextPage();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tutorialCollider != null)
        {
            if (collision.CompareTag("Player") && !tutorialSeen)
            {
                manager.inTutorial = true;

                currentPage = 0;

                tutorialPages[currentPage].gameObject.SetActive(true);
                StartCoroutine(FadeTutorial(tutorialPages[currentPage], 0f, 1f, 0.2f));

                // Play first video
                if (videoClips.Length > currentPage)
                {
                    player.clip = videoClips[currentPage];
                    player.Play();
                }

                tutorialSeen = true;
                tutorialOpen = true;

                manager.PauseTimeOnly();
            }
        }
    }

    private void NextPage()
    {
        StartCoroutine(SwitchPage());
    }

    private IEnumerator SwitchPage()
    {
        CanvasGroup current = tutorialPages[currentPage];

        current.alpha = 0f;
        current.gameObject.SetActive(false);

        currentPage++;

        if (currentPage >= tutorialPages.Length)
        {
            ClosePrompt();
            yield break;
        }

        CanvasGroup next = tutorialPages[currentPage];
        next.gameObject.SetActive(true);

        // Change video
        if (videoClips.Length > currentPage)
        {
            player.clip = videoClips[currentPage];
            player.Play();
        }

        next.alpha = 1f;
    }

    private void ClosePrompt()
    {
        player.Stop();

        foreach (CanvasGroup page in tutorialPages)
        {
            page.gameObject.SetActive(false);
        }

        tutorialOpen = false;

        manager.ResumeTimeOnly();
        manager.blockPauseInput = true;

        StartCoroutine(UnblockPause());

        manager.inTutorial = false;
    }

    private IEnumerator UnblockPause()
    {
        yield return null;
        manager.blockPauseInput = false;
    }

    private IEnumerator FadeTutorial(CanvasGroup group, float start, float end, float fadeTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, end, elapsedTime / fadeTime);
            yield return null;
        }

        group.alpha = end;
    }
}