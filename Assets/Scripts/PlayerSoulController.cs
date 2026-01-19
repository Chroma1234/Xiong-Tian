using UnityEngine;

public class PlayerSoulController : MonoBehaviour
{
    public Soul equippedYinSoul;
    public Soul equippedYangSoul;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void EquipYangSoul(Soul soul)
    {
        equippedYangSoul?.OnUnequip(player);
        equippedYangSoul = soul;
        soul.OnEquip(player);
    }

    public void ActivateYangSoul()
    {
        if(equippedYangSoul == null)
        {
            return;
        }

        if(player.Mana < equippedYangSoul.manaCost)
        {
            return;
        }

        player.Mana -= equippedYangSoul.manaCost;
        equippedYangSoul.OnActivate(player);
    }

    private void Update()
    {
        equippedYinSoul?.OnUpdate(player);
    }
}
