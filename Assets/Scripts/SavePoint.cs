using TMPro;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private bool playerInRange = false;
    private Player player;

    [SerializeField] private float saveCooldown = 1f;
    private float lastSaveTime = -Mathf.Infinity;

    [SerializeField] private GameObject saveText;

    [SerializeField] private ParticleSystem flameParticles;

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

            if (gm.currentSavePoint != null && gm.currentSavePoint != this)
            {
                gm.currentSavePoint.Deactivate();
            }

            // Set & enable this save point
            gm.currentSavePoint = this;
            gm.currentSpawnPoint = transform.position;
            Activate();


            gm.DoFlash();

            player.Health = 100;
            player.Mana = 100;

            player.SaveEffects();
            player.PlaySound(player.saveClip);
        }
    }

    public void Activate()
    {
        if (!flameParticles.isPlaying)
            flameParticles.Play();
    }

    public void Deactivate()
    {
        if (flameParticles.isPlaying)
            flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}