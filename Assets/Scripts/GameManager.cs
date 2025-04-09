using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // TMP UI Elements
    public TextMeshProUGUI cashText;           // Top left $Cash
    public TextMeshProUGUI passiveIncomeText;  // Bottom left passive income
    public TextMeshProUGUI employeeCountText;  // Bottom right employee count

    // Normal UI Text on Buttons
    public Text earnButtonText;
    public Text upgradeButtonText;
    public Text hireButtonText;

    // Buttons
    public Button earnButton;
    public Button upgradeButton;
    public Button hireButton;

    // Game Logic
    private float currentCash = 0f;

    private int incomePerClick = 1;
    private int upgradeLevel = 1;
    private int upgradeCost = 10;

    private int employeeCount = 0;
    private int employeeCost = 50;
    private float passiveIncomePerSec = 0f;

    void Start()
    {
        earnButton.onClick.AddListener(OnEarnClicked);
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        hireButton.onClick.AddListener(OnHireClicked);

        InvokeRepeating("AddPassiveIncome", 1f, 1f); // every 1 second
        UpdateUI();
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
            incomePerClick += 1;
            upgradeLevel++;
            upgradeCost = Mathf.RoundToInt(upgradeCost * 1.7f);
            UpdateUI();
        }
    }

    void OnHireClicked()
    {
        if (currentCash >= employeeCost)
        {
            currentCash -= employeeCost;
            employeeCount++;
            passiveIncomePerSec += 1f;
            employeeCost = Mathf.RoundToInt(employeeCost * 1.5f);
            UpdateUI();
        }
    }

    void AddPassiveIncome()
    {
        currentCash += passiveIncomePerSec;
        UpdateUI();
    }

    void UpdateUI()
    {
        // Top Cash Display
        cashText.text = "$" + currentCash.ToString("F0");

        // Button Labels
        earnButtonText.text = "EARN +$" + incomePerClick;
        upgradeButtonText.text = "UPGRADE ($" + upgradeCost + ")";
        hireButtonText.text = "HIRE ($" + employeeCost + ")";

        // Bottom Texts
        passiveIncomeText.text = "Passive: $" + passiveIncomePerSec + "/sec";
        employeeCountText.text = "Employee: " + employeeCount;
    }
}
