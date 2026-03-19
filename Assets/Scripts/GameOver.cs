using System.Collections;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private BoxCollider2D trigger;
    private Player player;
    private GameManager manager;
    [SerializeField] private TextMeshProUGUI thanks;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        trigger = GetComponent<BoxCollider2D>();
        manager = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.isTeleporting = true;
            StartCoroutine(manager.Fade(0f, 1f));
            thanks.gameObject.SetActive(true);
            StartCoroutine(Fade(0f, 1f, thanks, 0.5f));
        }
    }

    public IEnumerator Fade(float from, float to, TextMeshProUGUI text, float fadeDuration)
    {
        yield return new WaitForSeconds(1f);
        float timer = 0f;
        float a = text.alpha;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            a = Mathf.Lerp(from, to, timer / fadeDuration);
            text.alpha = a;
            yield return null;
        }

        a = to;
        text.alpha = a;
    }
}
