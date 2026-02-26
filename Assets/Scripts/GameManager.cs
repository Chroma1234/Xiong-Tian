using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region FPS
    [Header("FPS Control")]
    public int fps;
    #endregion

    #region Hitstop
    [Header("Hitstop Settings")]
    public static GameManager instance;
    private float defaultTimeScale = 1f;
    private float hitStopTimeScale = 0f;
    #endregion

    [SerializeField] private Image fadeImage;
    [SerializeField] private CanvasGroup flashImage;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float respawnDelay = 0.5f;
    [SerializeField] private float flashDuration;

    public SavePoint currentSavePoint;
    public Vector3 currentSpawnPoint;

    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioLowPassFilter filter;

    private readonly List<Enemy> enemies = new List<Enemy>();

    private float currentTimeScale = 1f;
    public bool paused = false;

    [SerializeField] private CanvasGroup pauseScreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = fps;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    private void Pause()
    {
        paused = true;
        currentTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        pauseScreen.alpha = 1f;
        pauseScreen.interactable = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        paused = false;
        Time.timeScale = currentTimeScale;

        pauseScreen.alpha = 0f;
        pauseScreen.interactable = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Fade(0f, 1f));
        yield return new WaitForSeconds(respawnDelay);

        Player player = FindFirstObjectByType<Player>();
        player.animator.SetBool("isAlive", true);
        player.Respawn(currentSpawnPoint);

        foreach (Enemy enemy in enemies)
        {
            enemy.ResetEnemy();
        }

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Fade(1f, 0f));
    }

    public IEnumerator Fade(float from, float to)
    {
        float timer = 0f;
        Color c = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, timer / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }

    private IEnumerator Flash()
    {
        flashImage.alpha = 0.5f;

        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            flashImage.alpha = Mathf.Lerp(0.5f, 0f, timer / flashDuration);
            yield return null;
        }

        flashImage.alpha = 0f;
    }
    public void DoFlash()
    {
        StartCoroutine(Flash());
    }

    public void DoHitStop(float duration)
    {
        StartCoroutine(HitStop(duration));
    }

    public void DoSlowDown(float factor, float duration)
    {
        StartCoroutine(SlowMotion(factor, duration));
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = hitStopTimeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = defaultTimeScale;
    }

    IEnumerator SlowMotion(float factor, float duration)
    {
        currentTimeScale = factor;

        if (!paused)
        {
            Time.timeScale = factor;
        }

        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        float t = 0f;
        while (t < 0.2f)
        {
            t += Time.unscaledDeltaTime;
            bgm.pitch = Mathf.Lerp(1f, 0.5f, t / 0.2f);
            filter.enabled = true;
            yield return null;
        }

        bgm.pitch = 0.5f;

        yield return new WaitForSecondsRealtime(duration);

        t = 0f;
        while (t < 0.2f)
        {
            t += Time.unscaledDeltaTime;
            bgm.pitch = Mathf.Lerp(0.5f, 1f, t / 0.2f);
            filter.enabled = false;
            yield return null;
        }

        bgm.pitch = 1;

        currentTimeScale = 1f;

        if (!paused)
        {
            Time.timeScale = currentTimeScale;
        }

        Time.fixedDeltaTime = 0.02f;
    }

}
