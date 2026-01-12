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
    #endregion

    private void Awake()
    {
        player = GetComponent<Player>();
        damageable = GetComponent<IDamageable>();
    }

    private void Update()
    {
        healthText.text = "Health: " + damageable.Health.ToString();
    }

}
