using TMPro;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private bool playerInRange = false;
    private Player player;

    [SerializeField] private float saveCooldown = 1f;
    private float lastSaveTime = -Mathf.Infinity;

    [SerializeField] private GameObject saveText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player p))
        {
            playerInRange = true;
            player = p;
            saveText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player p) && p == player)
        {
            playerInRange = false;
            player = null;
            saveText.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            if (Time.time - lastSaveTime < saveCooldown)
                return;

            lastSaveTime = Time.time;

            GameManager gm = FindFirstObjectByType<GameManager>();
            gm.currentSpawnPoint = transform.position;
            gm.DoFlash();

            player.Health = 100;
            player.Mana = 100;

            player.SaveEffects();
            player.PlaySound(player.saveClip);
        }
    }
}