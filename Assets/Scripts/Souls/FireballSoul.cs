using UnityEngine;

[CreateAssetMenu(menuName = "Souls/Yang/Fireball")]
public class FireballSoul : Soul
{
    public GameObject fireballPrefab;
    public float speed;

    public override void OnActivate(Player player)
    {
        Vector2 dir = player.lastFacingDirection;

        GameObject fb = Instantiate(
            fireballPrefab,
            player.transform.position,
            Quaternion.identity
        );

        fb.GetComponent<Rigidbody2D>().linearVelocity = dir * speed;
    }
}

