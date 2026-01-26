using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Component References
    [Header("Component References")]
    [SerializeField] Player player;
    IDamageable damageable;
    #endregion

    #region Text UI
    [Header("Text UI")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI manaText;
    [SerializeField] TextMeshProUGUI staminaText;
    [SerializeField] TextMeshProUGUI parryChargeText;
    #endregion

    private void Awake()
    {
        player = GetComponent<Player>();
        damageable = GetComponent<IDamageable>();
    }

    private void Update()
    {
        healthText.text = "Health: " + damageable.Health.ToString();
        manaText.text = "Mana: " + player.Mana.ToString();
        staminaText.text = "Dashes: " + player.dashCount.ToString();
        parryChargeText.text = "Parry Charge: " + player.hasParryCharge.ToString();
    }

}
