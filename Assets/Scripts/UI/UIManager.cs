using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Component References
    [Header("Component References")]
    [SerializeField] Player player;
    [SerializeField] Boss boss;
    private Camera cam;
    #endregion

    #region Player UI
    [Header("Player UI")]
    [SerializeField] private RectTransform dashBarUI;
    [SerializeField] private CanvasGroup dashBarCanvasGrp;
    [SerializeField] private Image dashFill;
    [SerializeField] Image healthBar;
    [SerializeField] Image lowHealthOverlay;
    
    private bool lowHealthActive;
    #endregion

    #region Boss UI
    [Header("Boss UI")]
    [SerializeField] Image bossHealthBar;
    [SerializeField] CanvasGroup bossUI;
    #endregion

    #region Low Health Pulse Settings
    [Header("Low Health Pulse")]
    [SerializeField] private float lowHealthPulseSpeed = 2f;
    [SerializeField] private float lowHealthMinAlpha = 0.3f;
    [SerializeField] private float lowHealthMaxAlpha = 0.7f;
    #endregion

    private void Awake()
    {
        player = GetComponent<Player>();
        boss = FindFirstObjectByType<Boss>();
        
        cam = Camera.main;
    }

    private void Start()
    {
        player.OnHealthChanged += OnHealthChanged;
        boss.OnHealthChanged += OnBossHealthChanged;
        Boss.OnBossKilled += OnBossKilled;
    }

    private void Update()
    {
        dashFill.fillAmount = Mathf.Lerp(dashFill.fillAmount, (float)player.dashCount / player.maxDashCount, 10f * Time.deltaTime);
        dashBarCanvasGrp.alpha = Mathf.Lerp(dashBarCanvasGrp.alpha, dashFill.fillAmount < 0.99f ? 1f : 0f, 8f * Time.deltaTime);

        if (player.freezePlayer)
        {
            dashBarCanvasGrp.alpha = 0;
        }

        if (lowHealthActive)
        {
            Color c = lowHealthOverlay.color;

            float alpha = Mathf.Lerp(
                lowHealthMinAlpha,
                lowHealthMaxAlpha,
                Mathf.PingPong(Time.time * lowHealthPulseSpeed, 1)
            );

            c.a = alpha;
            lowHealthOverlay.color = c;
        }
    }

    private void LateUpdate()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(player.transform.position + new Vector3(0, 1.3f, 0));
        dashBarUI.position = screenPos;
    }

    private void OnDestroy()
    {
        player.OnHealthChanged -= OnHealthChanged;
        boss.OnHealthChanged -= OnBossHealthChanged;
        Boss.OnBossKilled -= OnBossKilled;
    }

    private void OnHealthChanged(int health)
    {
        // Update health bar fill
        healthBar.fillAmount = health / 100f;

        bool shouldShowLowHealth = health <= 30;

        if (shouldShowLowHealth != lowHealthActive)
        {
            lowHealthActive = shouldShowLowHealth;

            // Fade overlay
            if (!lowHealthActive)
            {
                // Fade overlay out when health is safe
                StartCoroutine(Fade(lowHealthOverlay.color.a, 0f, 0.5f, lowHealthOverlay));
            }
        }
    }

    private void OnBossHealthChanged(int health)
    {
        bossHealthBar.fillAmount = (float)health / 350f;
    }

    private void OnBossKilled(Boss boss)
    {
        StartCoroutine(Fade(1f, 0f, 0.5f, bossUI));
    }

    private IEnumerator Fade(float from, float to, float duration, Image image)
    {
        float timer = 0f;
        Color c = image.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, timer / duration);
            image.color = c;
            yield return null;
        }

        c.a = to;
        image.color = c;
    }

    private IEnumerator Fade(float from, float to, float duration, CanvasGroup image)
    {
        float timer = 0f;
        float c = image.alpha;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            c = Mathf.Lerp(from, to, timer / duration);
            image.alpha = c;
            yield return null;
        }

        c = to;
        image.alpha = c;
    }
}
