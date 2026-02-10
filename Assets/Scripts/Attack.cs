using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float knockbackForce = 8f;

    public bool blockable;
    public bool parryable;

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
                            hit.TakeHit(attackDamage, hitDirection, knockbackForce);
                        }
                    }
                }
                else
                {
                    hit.TakeHit(attackDamage, hitDirection, knockbackForce);
                }
            }
        }
    }
}
