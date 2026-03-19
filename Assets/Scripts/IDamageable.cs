using UnityEngine;

public interface IDamageable
{
    public int Health {  get; set; }
    public bool IsAlive { get; set; }
    void TakeHit(int damage, Vector2 hitDirection, float knockbackForce);
}

