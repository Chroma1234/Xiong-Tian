using TMPro;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    #region Component References
    private bool playerInRange = false;
    private Player player;

    private GameManager gm;
    #endregion

    #region Save FX
    [Header("Save Effects")]
    [SerializeField] private GameObject saveText;
    [SerializeField] private ParticleSystem flameParticles;
    [SerializeField] private GameObject saveLighting;
    #endregion

    private void Awake()
    {
        gm = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player p))
        {
            playerInRange = true;
            player = p;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player p) && p == player)
        {
            playerInRange = false;
            player = null;
        }
    }

    private void Update()
    {
        if (playerInRange && gm.currentSavePoint != this)
        {
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

            player.SaveEffects();
            player.PlaySound(player.saveClip);
        }
    }

    public void Activate()
    {
        if (!flameParticles.isPlaying)
        {
            flameParticles.Play();
            saveLighting.SetActive(true);
        }
    }

    public void Deactivate()
    {
        if (flameParticles.isPlaying)
        {
            flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            saveLighting.SetActive(false);
        }
    }
}