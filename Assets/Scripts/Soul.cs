using UnityEngine;

public enum SoulType
{
    Yin,
    Yang
}

public abstract class Soul : ScriptableObject
{
    public string soulName;
    public SoulType soulType;
    public string soulDescription;
    //public Sprite icon;

    public int manaCost;
    public float cooldown;

    public virtual void OnEquip(Player player) { }
    public virtual void OnUnequip(Player player) { }
    public virtual void OnActivate(Player player) { }
    public virtual void OnUpdate(Player player) { }
}
