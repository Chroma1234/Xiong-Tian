using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float knockbackForce = 8f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable hit))
        {
            Vector2 hitDirection =
                (collision.transform.position - transform.position).normalized;

            hit.TakeHit(attackDamage, hitDirection, knockbackForce);

            Debug.Log($"{collision.name} hit for {attackDamage}");
        }
    }
}
