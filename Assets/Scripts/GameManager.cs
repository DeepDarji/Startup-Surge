using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // UI References
    public TextMeshProUGUI cashText;           // Text that shows current cash (e.g., "$50")
    public TextMeshProUGUI incomeText;         // Text that shows current income per click (e.g., "+$1 per click")
    public Button earnButton;
    public Button upgradeButton;

    // Game Logic
    private float currentCash = 0f;
    private int incomePerClick = 1;
    private int upgradeLevel = 1;
    private int upgradeCost = 10;

    void Start()
    {
        UpdateUI();

        // Optional: Hook buttons if not connected in inspector
        earnButton.onClick.AddListener(OnEarnClicked);
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
    }

    void OnEarnClicked()
    {
        currentCash += incomePerClick;
        UpdateUI();
    }

    void OnUpgradeClicked()
    {
        if (currentCash >= upgradeCost)
        {
            currentCash -= upgradeCost;
            upgradeLevel++;
            incomePerClick += 1;
            upgradeCost = Mathf.RoundToInt(upgradeCost * 1.7f); // exponential cost

            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough money to upgrade!");
        }
    }

    void UpdateUI()
    {
        cashText.text = "$" + currentCash.ToString("F0");
        incomeText.text = "+$" + incomePerClick + " per click\nUpgrade: $" + upgradeCost;
    }
}
