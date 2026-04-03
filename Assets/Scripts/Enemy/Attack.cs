using UnityEngine;

public class Attack : MonoBehaviour
{
    #region Attack Settings
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float knockbackForce = 8f;
    public bool parryable;
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObj = collision.gameObject;
        if (collision.TryGetComponent(out IDamageable hit))
        {
            Vector2 hitDirection = (collision.transform.position - transform.position).normalized;

            if (gameObj.GetComponent<Player>() != null)
            {
                Player player = gameObj.GetComponent<Player>();
                if (player.parry && parryable)
                {
                    Enemy enemy = GetComponentInParent<Enemy>();
                    Boss boss = GetComponentInParent<Boss>();

                    if (enemy != null)
                    {
                        Vector2 directionToEnemy = (enemy.transform.position - player.transform.position).normalized;
                        Vector2 playerForward;

                        if(player.transform.localScale.x > 0)
                        {
                            playerForward = Vector2.right;
                        }
                        else
                        {
                            playerForward = Vector2.left;
                        }

                        float dotProduct = Vector2.Dot(playerForward, directionToEnemy);

                        if(dotProduct >= 0.7f)
                        {
                            player.Parry();
                            player.PlaySound(player.parryClip);
                            player.PlaySound(player.impactClip);

                            enemy.ParryKnockback(hitDirection);
                            enemy.StateMachine.ChangeState(enemy.StunnedState);
                        }
                        else
                        {
                            hit.TakeHit(attackDamage, hitDirection, knockbackForce, false);
                        }
                    }

                    else if (boss != null)
                    {
                        Vector2 directionToBoss = (boss.transform.position - player.transform.position).normalized;
                        Vector2 playerForward;

                        if (player.transform.localScale.x > 0)
                        {
                            playerForward = Vector2.right;
                        }
                        else
                        {
                            playerForward = Vector2.left;
                        }

                        float dotProduct = Vector2.Dot(playerForward, directionToBoss);

                        if (dotProduct >= 0.7f)
                        {
                            player.Parry();
                            player.PlaySound(player.parryClip);
                            player.PlaySound(player.impactClip);

                            boss.ParryKnockback(hitDirection);
                            boss.StateMachine.ChangeState(boss.StunnedState);
                        }
                        else
                        {
                            hit.TakeHit(attackDamage, hitDirection, knockbackForce, false);
                        }
                    }
                }
                else
                {
                    hit.TakeHit(attackDamage, hitDirection, knockbackForce, false);
                }
            }
        }
    }
}
