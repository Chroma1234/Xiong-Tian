using System;
using System.Collections;
using System.Xml;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public static event Action<Enemy> OnEnemyHit;
    public static event Action<Enemy> OnEnemyKilled;

    [SerializeField] GameManager manager;
    [SerializeField] CameraController cam;
    SpriteRenderer spriteRenderer;

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health;

    [SerializeField] private float knockbackForce = 8f;

    [SerializeField] private float hitFlashDuration;

    public int Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);

            if (health <= 0 && IsAlive)
            {
                //death logic
                manager.DoHitStop();
                cam.DoShake();

                IsAlive = false;

                OnEnemyKilled?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }

    public bool IsAlive { get; set; } = true;

    public void TakeHit(int damage, Vector2 hitDirection, float knockbackForce)
    {
        if (!IsAlive)
            return;

        Health -= damage;

        OnEnemyHit?.Invoke(this);

        // Enter hit state (unless dead)
        if (Health > 0)
        {
            manager.DoHitStop();
            cam.DoShake();

            StartCoroutine(FlashSprite());
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cam = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    private void Start()
    {
        health = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.layer == LayerMask.NameToLayer("Attack"))
        {
            int dmgTaken = obj.GetComponent<Weapon>().weaponDamage;
            TakeHit(dmgTaken, Vector2.zero, knockbackForce);
        }
    }

    private IEnumerator FlashSprite()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }
}

