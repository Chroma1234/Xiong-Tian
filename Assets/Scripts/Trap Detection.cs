using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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
    [SerializeField] private GameObject heavensgate;
    [SerializeField] private SpriteRenderer[] bossBGSprites;

    private GameManager gameManager;
    [SerializeField] private AudioSource bgm;

    [SerializeField] private AudioClip bossMusic;

    private Player player;

    [SerializeField] private GameObject bossHealthbar;

    private Camera cam;
    private CameraController camController;

    private void Awake()
    {
        trapdoor.gameObject.SetActive(false);
        gameManager = FindFirstObjectByType<GameManager>();
        cam = Camera.main;
        camController = cam.GetComponent<CameraController>();

        if (heavensgate != null)
        {
            heavensgate.gameObject.SetActive(false);

        }
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

            heavensgate.gameObject.SetActive(false);

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
        player.freezePlayer = true;

        yield return StartCoroutine(gameManager.Fade(0f, 1f));
        yield return StartCoroutine(FadeAudio(0f, 0.5f));
        player.gameObject.transform.position = new Vector2(bossTeleport.position.x, player.transform.position.y);

        player.isTeleporting = true;
        player.rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);
        caveBG.gameObject.SetActive(false);
        
        if (heavensgate != null)
        {
            heavensgate.gameObject.SetActive(true);

        }

        foreach (SpriteRenderer renderer in bossBGSprites)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);
        }

        bgm.clip = bossMusic;
        bgm.Play();

        yield return StartCoroutine(FadeAudio(0.3f, 0.5f));
        yield return StartCoroutine(gameManager.Fade(1f, 0f));
        //player.isTeleporting = false;
        StartCoroutine(CutsceneRoutine());
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

    private IEnumerator CutsceneRoutine()
    {
        camController.inCutscene = true;
        player.isTeleporting = true;
        player.rb.linearVelocity = Vector2.zero;

        Vector3 originalPos = cam.transform.position;
        Vector3 camPos = new Vector3(465.64f, 42.45f, cam.transform.position.z);
        float elapsedTime = 0;

        while (elapsedTime < 0.5f)
        {
            float t = elapsedTime / 0.5f;
            cam.transform.position = Vector3.Lerp(cam.transform.position, camPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = camPos;
        elapsedTime = 0;
        boss.GetComponent<Boss>().StartCoroutine(boss.GetComponent<Boss>().Entrance());

        while (elapsedTime < 0.5f)
        {
            float t = elapsedTime / 0.5f;
            Color originalColor = boss.GetComponent<SpriteRenderer>().color;
            boss.GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 1f), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;
        yield return new WaitForSeconds(1.2f);

        while (elapsedTime < 0.5f)
        {
            float t = elapsedTime / 0.5f;
            cam.transform.position = Vector3.Lerp(cam.transform.position, originalPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = originalPos;

        camController.inCutscene = false;
        player.isTeleporting = false;

        bossHealthbar.SetActive(true);
        StartCoroutine(Fade(0f, 1f, bossHealthbar.GetComponent<CanvasGroup>(), 0.5f));

        camController.inCutscene = false;
        player.freezePlayer = false; // unfreeze player now
        player.isTeleporting = false;
    }

}
