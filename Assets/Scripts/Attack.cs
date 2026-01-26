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
                if (gameObj.GetComponent<Player>().parry && parryable)
                {
                    player.StartCoroutine(player.Parry());
                    Destroy(transform.gameObject); // temp
                    Debug.Log("parried!");
                }
                else if(gameObj.GetComponent<Player>().StateMachine.CurrentPlayerState == player.BlockState && blockable)
                {
                    Debug.Log("blocked!");
                }
                else
                {
                    hit.TakeHit(attackDamage, hitDirection, knockbackForce);
                }
            }
        }
    }
}
