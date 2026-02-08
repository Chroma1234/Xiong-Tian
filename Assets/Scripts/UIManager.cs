using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Component References
    [Header("Component References")]
    [SerializeField] Player player;
    #endregion

    #region Text UI
    [Header("Text UI")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI manaText;
    [SerializeField] TextMeshProUGUI staminaText;
    [SerializeField] TextMeshProUGUI parryChargeText;
    [SerializeField] Image healthBar;
    [SerializeField] Image manaBar;
    #endregion

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        player.OnHealthChanged += health => healthBar.fillAmount = health / 100f;
        player.OnManaChanged += mana => manaBar.fillAmount = mana / 100f;
    }

    private void Update()
    {
        staminaText.text = "Dashes: " + player.dashCount.ToString();
        parryChargeText.text = "Parry Charge: " + player.hasParryCharge.ToString();
    }

}
