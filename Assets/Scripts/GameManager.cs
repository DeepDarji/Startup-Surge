using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // TMP UI Elements
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI passiveIncomeText;
    public TextMeshProUGUI employeeCountText;
    public TextMeshProUGUI popupText;  // NEW: popup text

    // Button Texts
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

    public PopupManager popupManager;

    void Start()
    {
        earnButton.onClick.AddListener(OnEarnClicked);
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        hireButton.onClick.AddListener(OnHireClicked);

        InvokeRepeating("AddPassiveIncome", 1f, 1f);
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
            popupManager.ShowPopup("Upgrade successful!");
            UpdateUI();
        }
        else
        {
            ShowPopup("Not enough money! Need $" + upgradeCost);
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
            popupManager.ShowPopup("Hired! Total Employees: " + employeeCount);
            UpdateUI();

            ShowPopup("Employee hired! Total: " + employeeCount);
        }
        else
        {
            ShowPopup("Not enough money! Need $" + employeeCost);
        }
    }

    void AddPassiveIncome()
    {
        currentCash += passiveIncomePerSec;
        UpdateUI();
    }

    void UpdateUI()
    {
        cashText.text = "$" + currentCash.ToString("F0");
        earnButtonText.text = "EARN +$" + incomePerClick;
        upgradeButtonText.text = "UPGRADE ($" + upgradeCost + ")";
        hireButtonText.text = "HIRE ($" + employeeCost + ")";
        passiveIncomeText.text = "Passive: $" + passiveIncomePerSec + "/sec";
        employeeCountText.text = "Emp.: " + employeeCount;
    }

    //  NEW: Shows popup text and fades out
    void ShowPopup(string message)
    {
        StopAllCoroutines(); // if a previous popup is still fading
        popupText.text = message;
        popupText.alpha = 1f;
        StartCoroutine(FadePopup());
    }

    System.Collections.IEnumerator FadePopup()
    {
        yield return new WaitForSeconds(2f);

        float fadeTime = 1f;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            popupText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        popupText.alpha = 0f;
    }
}
