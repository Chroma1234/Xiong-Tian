using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.layer == LayerMask.NameToLayer("Attack"))
        {
            Weapon weapon = obj.GetComponent<Weapon>();
            int dmgTaken = weapon.weaponDamage;
            Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
            enemy.TakeHit(dmgTaken, hitDirection, enemy.knockbackForce);

            if (weapon.projectile)
            {
                Destroy(obj);
            }
        }
    }
}
