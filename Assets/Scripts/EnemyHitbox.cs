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
            Player player = obj.GetComponentInParent<Player>();
            Weapon weapon = obj.GetComponent<Weapon>();
            if (player.hasParryCharge)
            {
                int dmgTaken = weapon.weaponDamage * 5;
                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                enemy.TakeHit(dmgTaken, hitDirection, enemy.knockbackForce);
                
                player.hasParryCharge = false;
                Debug.Log("used parry charge!");
                //add other fx
            }
            else
            {
                int dmgTaken = weapon.weaponDamage;
                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                enemy.TakeHit(dmgTaken, hitDirection, enemy.knockbackForce);
            }
            
            if (weapon.projectile)
            {
                Destroy(obj);
            }
        }
    }


}
