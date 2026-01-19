using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int weaponDamage;
    public bool projectile;
    public float lifetime;

    private float timer = 0f;

    private void Update()
    {
        if (lifetime != 0f && projectile)
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
