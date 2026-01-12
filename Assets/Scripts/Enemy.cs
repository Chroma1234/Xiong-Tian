using System.Collections;
using System.Xml;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
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
                IsAlive = false;

                Destroy(gameObject);
                Debug.Log("dead");
            }
        }
    }

    public bool IsAlive { get; set; } = true;

    public void TakeHit(int damage, Vector2 hitDirection, float knockbackForce)
    {
        if (!IsAlive)
            return;

        Health -= damage;

        // Enter hit state (unless dead)
        if (Health > 0)
        {
            manager.DoHitStop();
            cam.DoShake();
            StartCoroutine(FlashSprite());
        }
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.layer == LayerMask.NameToLayer("Attack"))
        {
            int dmgTaken = obj.GetComponent<Weapon>().weaponDamage;
            TakeHit(dmgTaken, Vector2.zero, knockbackForce);

            Debug.Log("hit!");
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

