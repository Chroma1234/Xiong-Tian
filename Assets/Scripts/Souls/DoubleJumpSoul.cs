using UnityEngine;

[CreateAssetMenu(menuName = "Souls/Yang/DoubleJump")]
public class DoubleJumpSoul : Soul
{
    public override void OnEquip(Player player)
    {
        player.doubleJump = true;
    }

    public override void OnUnequip(Player player)
    {
        player.doubleJump = false;
    }
}

