using System.Collections;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    GameManager gameManager;
    public Transform teleportLocation;

    [SerializeField] private GameObject oldBG;
    [SerializeField] private GameObject newBG;

    [SerializeField] private SpriteRenderer[] fog;

    [SerializeField] private GameObject playerLight;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObj = collision.gameObject;
        if (gameObj != null)
        {
            if(gameObj.GetComponent<Player>() != null)
            {
                Player player = gameObj.GetComponent<Player>();
                player.isTeleporting = true;
                player.rb.linearVelocity = Vector2.zero;
                StartCoroutine(TeleportToCave(gameObj));
            }
        }
    }

    private IEnumerator TeleportToCave(GameObject player)
    {
        yield return StartCoroutine(gameManager.Fade(0f, 1f));
        player.transform.position = teleportLocation.position;
        yield return new WaitForSeconds(0.5f);

        oldBG.SetActive(false);
        newBG.SetActive(true);

        foreach (SpriteRenderer a in fog)
        {
            Color fogColor = a.color;
            a.color = new Color(fogColor.r, fogColor.g, fogColor.b, 0.03f);
        }

        playerLight.SetActive(true);

        yield return StartCoroutine(gameManager.Fade(1f, 0f));
        player.GetComponent<Player>().isTeleporting = false;
    }
}
