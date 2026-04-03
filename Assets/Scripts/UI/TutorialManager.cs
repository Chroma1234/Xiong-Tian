using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    #region Component References
    [Header("Component References")]
    [SerializeField] private GameManager manager;
    [SerializeField] private VideoPlayer player;
    #endregion

    #region Tutorial Trigger
    private BoxCollider2D tutorialCollider;
    #endregion

    #region Tutorial UI
    [SerializeField] private CanvasGroup[] tutorialPages;
    [SerializeField] private VideoClip[] videoClips;
    #endregion

    #region Misc
    private int currentPage = 0;

    private bool tutorialSeen = false;
    private bool tutorialOpen = false;
    #endregion

    private void Awake()
    {
        tutorialCollider = GetComponent<BoxCollider2D>();

        foreach (CanvasGroup page in tutorialPages)
        {
            page.alpha = 0f;
            page.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!tutorialOpen) return;

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
        manager.inTutorial = false;
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