using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    Enemy enemy;
    Boss boss;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        boss = GetComponentInParent<Boss>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.layer == LayerMask.NameToLayer("Attack"))
        {
            if (enemy != null)
            {
                Player player = obj.GetComponentInParent<Player>();
                Weapon weapon = obj.GetComponent<Weapon>();
                if (player.hasParryCharge)
                {
                    int dmgTaken = weapon.weaponDamage * 3;
                    Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                    enemy.TakeHit(dmgTaken, hitDirection, enemy.knockbackForce, false);

                    player.hasParryCharge = false;
                    Debug.Log("used parry charge!");
                    //add other fx
                }

                else if (!enemy.shieldType)
                {
                    int dmgTaken = weapon.weaponDamage;
                    Vector2 hitDirection = (collision.transform.position - transform.position).normalized;

                    enemy.TakeHit(dmgTaken, hitDirection, enemy.knockbackForce, false);
                }

                else if (enemy.shieldType)
                {
                    int dmgTaken = 0;
                    Vector2 hitDirection = (collision.transform.position - transform.position).normalized;

                    enemy.TakeHit(dmgTaken, hitDirection, enemy.knockbackForce, true);
                }
            }

            if (boss != null)
            {
                Player player = obj.GetComponentInParent<Player>();
                Weapon weapon = obj.GetComponent<Weapon>();
                Debug.Log("Weapon damage: " + weapon.weaponDamage);
                if (player.hasParryCharge)
                {
                    int dmgTaken = weapon.weaponDamage;
                    Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                    boss.TakeHit(dmgTaken, hitDirection, boss.knockbackForce, false);

                    player.hasParryCharge = false;
                    Debug.Log("used parry charge!");
                    //add other fx
                }

                else
                {
                    int dmgTaken = weapon.weaponDamage;
                    Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                    boss.TakeHit(dmgTaken, hitDirection, boss.knockbackForce, false);
                }
            }
        }
    }


}
