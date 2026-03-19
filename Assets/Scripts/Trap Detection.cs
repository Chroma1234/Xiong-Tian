using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class Trap_Detection : MonoBehaviour
{

    [SerializeField] 
    public BoxCollider2D trap_detection;
    public GameObject trapdoor;
    public Transform bossTeleport;

    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject caveBG;
    [SerializeField] private SpriteRenderer[] bossBGSprites;

    private GameManager gameManager;
    [SerializeField] private AudioSource bgm;

    [SerializeField] private AudioClip bossMusic;

    private Player player;

    [SerializeField] private GameObject bossHealthbar;

    private void Awake()
    {
        trap_detection = GetComponent<BoxCollider2D>();
        trapdoor.gameObject.SetActive(false);
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (player != null && !player.IsAlive)
        {
            if (trapdoor != null)
            {
                trapdoor.SetActive(false);
            }
                trap_detection.enabled = true;
                //Debug.Log("Player died");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            trap_detection.enabled = false;
            StartCoroutine(Appear());

            player = collision.gameObject.GetComponent<Player>();

            if (boss != null)
            {
                player.isTeleporting = true;
                player.rb.linearVelocity = Vector2.zero;
                StartCoroutine(Setup(player));
            }
        }
    }

    private IEnumerator Appear()
    {
        trapdoor.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < 0.75f)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(1.1f, 0, (elapsedTime / 0.75f));

            trapdoor.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_DissolveAmt"), lerpedDissolve);

            yield return null;
        }
        trapdoor.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_DissolveAmt"), 0);
    }

    private IEnumerator Setup(Player player)
    {
        yield return StartCoroutine(gameManager.Fade(0f, 1f));
        yield return StartCoroutine(FadeAudio(0f, 0.5f));
        player.gameObject.transform.position = new Vector2(bossTeleport.position.x, player.transform.position.y);
        yield return new WaitForSeconds(0.5f);
        caveBG.SetActive(false);

        foreach (SpriteRenderer renderer in bossBGSprites)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);
        }

        boss.SetActive(true);
        bgm.clip = bossMusic;
        bgm.Play();

        yield return StartCoroutine(FadeAudio(0.1f, 0.5f));
        yield return StartCoroutine(gameManager.Fade(1f, 0f));
        player.isTeleporting = false;

        bossHealthbar.SetActive(true);
        StartCoroutine(Fade(0f, 1f, bossHealthbar.GetComponent<CanvasGroup>(), 0.5f));
    }

    private IEnumerator FadeAudio(float targetVolume, float duration)
    {
        float startVolume = bgm.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgm.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        bgm.volume = targetVolume;
    }

    public IEnumerator Fade(float from, float to, CanvasGroup fadeImage, float fadeDuration)
    {
        float timer = 0f;
        float a = fadeImage.alpha;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            a = Mathf.Lerp(from, to, timer / fadeDuration);
            fadeImage.alpha = a;
            yield return null;
        }

        a = to;
        fadeImage.alpha = a;
    }
}
