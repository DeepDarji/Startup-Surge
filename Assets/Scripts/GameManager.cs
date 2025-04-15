using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ========================= UI Elements =========================
    public TextMeshProUGUI cashText;              // Text showing player's current cash
    public TextMeshProUGUI passiveIncomeText;     // Text showing passive income rate
    public TextMeshProUGUI employeeCountText;     // Text showing number of employees hired

    public Text earnButtonText;                   // Text on the "Earn" button
    public Text upgradeButtonText;                // Text on the "Upgrade" button
    public Text hireButtonText;                   // Text on the "Hire" button

    public TextMeshProUGUI popupText;             // Floating popup text for feedback

    // ========================= Buttons =========================
    public Button earnButton;                     // Earn money on click
    public Button upgradeButton;                  // Upgrade income per click
    public Button hireButton;                     // Hire new employees

    // ========================= Game Values =========================
    public float currentCash = 999999f;           // Starting money
    public int incomePerClick = 1;                // How much money you earn per click
    public int upgradeLevel = 1;                  // Level of income upgrade
    public int upgradeCost = 10;                  // Cost of upgrading

    public int employeeCount = 0;                 // Number of employees hired
    public int employeeCost = 50;                 // Cost of hiring new employee
    public float passiveIncome = 0f;              // Total passive income from employees

    // ========================= Employees =========================
    public GameObject[] employeePrefabs;          // Different employee types/prefabs
    public Transform employeeParent;              // Parent object for spawned employees

    // ========================= Camera =========================
    public Camera mainCamera;                     // Main camera for movement
    public Vector3 cameraLeftLimit = new Vector3(-6f, 0f, -10f);  // Camera minimum X
    public Vector3 cameraRightLimit = new Vector3(6f, 0f, -10f);  // Camera maximum X

    // ========================= Audio =========================
    public AudioSource audioSource;               // Main audio source
    public AudioClip earnClip, upgradeClip, hireClip, errorClip;  // Sounds for actions

    // ========================= Helpers =========================
    public List<Vector3> filledPositions = new List<Vector3>();   // Already used positions

    public GameEnd gameEndManager;                // Reference to GameEnd class to end game

    // ========================= Start Function =========================
    void Start()
    {
        // Attach functions to button clicks
        earnButton.onClick.AddListener(EarnMoney);
        upgradeButton.onClick.AddListener(UpgradeClick);
        hireButton.onClick.AddListener(HireEmployee);

        // Start passive income loop every second
        InvokeRepeating("GivePassiveIncome", 1f, 1f);

        popupText.gameObject.SetActive(false); // Hide popup by default

        RefreshUI(); // Initialize UI values
    }

    // ========================= Money Earning =========================
    void EarnMoney()
    {
        currentCash += incomePerClick; // Add click income
        PlaySound(earnClip); // Play sound
        RefreshUI(); // Update UI
    }

    // ========================= Upgrading =========================
    void UpgradeClick()
    {
        if (currentCash >= upgradeCost)
        {
            // Upgrade if enough money
            currentCash -= upgradeCost;
            incomePerClick++;
            upgradeLevel++;
            upgradeCost = Mathf.RoundToInt(upgradeCost * 1.7f); // Increase cost

            ShowPopup("Upgrade successful!");
            PlaySound(upgradeClip);
            RefreshUI();
        }
        else
        {
            // Show error if not enough money
            ShowPopup("Low Money! Need $" + upgradeCost);
            PlaySound(errorClip);
        }
    }

    // ========================= Hiring Employees =========================
    void HireEmployee()
    {
        if (currentCash >= employeeCost)
        {
            Vector3 position;
            if (FindFreePosition(out position))
            {
                currentCash -= employeeCost;
                employeeCount++;
                passiveIncome += 1f;
                employeeCost = Mathf.RoundToInt(employeeCost * 1.5f); // Cost increases

                GameObject employeeToSpawn = null;

                // Choose which employee prefab to use based on position
                if (position.z == 0f)
                {
                    if (position.x == -7.5f || position.x == -5f || position.x == -2.5f)
                        employeeToSpawn = employeePrefabs[2]; // left-most gets a special prefab
                    else
                        employeeToSpawn = employeePrefabs[4]; // right side
                }
                else
                {
                    // Random prefab for zone B
                    int r = Random.Range(0, 3);
                    if (r == 0)
                        employeeToSpawn = employeePrefabs[0];
                    else if (r == 1)
                        employeeToSpawn = employeePrefabs[1];
                    else
                        employeeToSpawn = employeePrefabs[3];
                }

                // Spawn employee if prefab is valid
                if (employeeToSpawn != null)
                {
                    Instantiate(employeeToSpawn, position, Quaternion.identity, employeeParent);
                    filledPositions.Add(position);

                    ShowPopup("Employee Hired! Total: " + employeeCount);
                    PlaySound(hireClip);
                    MoveCamera(position.x);
                    RefreshUI();
                }
                else
                {
                    ShowPopup("Invalid employee prefab.");
                    PlaySound(errorClip);
                }
            }
            else
            {
                ShowPopup("No space left to hire!");
                PlaySound(errorClip);
            }
        }
        else
        {
            ShowPopup("Low Money! Need $" + employeeCost);
            PlaySound(errorClip);
        }

        // Trigger game end if 15 employees are hired
        if (employeeCount >= 15)
        {
            gameEndManager.EndGame();
        }
    }

    // ========================= Passive Income =========================
    void GivePassiveIncome()
    {
        currentCash += passiveIncome; // Add passive income per second
        RefreshUI(); // Update UI
    }

    // ========================= UI Update =========================
    void RefreshUI()
    {
        // Update all UI texts
        cashText.text = "$" + currentCash.ToString("F0");
        earnButtonText.text = "EARN +$" + incomePerClick;
        upgradeButtonText.text = "UPGRADE ($" + upgradeCost + ")";
        hireButtonText.text = "HIRE ($" + employeeCost + ")";
        passiveIncomeText.text = "Passive: $" + passiveIncome + "/sec";
        employeeCountText.text = "Emp.: " + employeeCount;
    }

    // ========================= Popup Animation =========================
    void ShowPopup(string msg)
    {
        popupText.text = msg;
        popupText.gameObject.SetActive(true);
        popupText.transform.localScale = Vector3.one * 0.8f;
        popupText.alpha = 1f;

        StopCoroutine("AnimatePopup"); // Stop any current animation
        StartCoroutine(AnimatePopup()); // Start new animation
    }

    IEnumerator AnimatePopup()
    {
        float t = 0f;
        float duration = 2f;

        Vector3 start = Vector3.one * 0.8f;
        Vector3 end = Vector3.one;

        while (t < duration)
        {
            t += Time.deltaTime;
            float percent = t / duration;

            popupText.transform.localScale = Vector3.Lerp(start, end, percent);
            popupText.alpha = Mathf.Lerp(1f, 0f, percent);

            yield return null;
        }

        popupText.gameObject.SetActive(false); // Hide after fade
    }

    // ========================= Camera Control =========================
    void MoveCamera(float x)
    {
        // Clamp target X to limits
        Vector3 target = new Vector3(Mathf.Clamp(x, cameraLeftLimit.x, cameraRightLimit.x), 0f, -10f);
        StopAllCoroutines();
        StartCoroutine(MoveCameraRoutine(target));
    }

    IEnumerator MoveCameraRoutine(Vector3 target)
    {
        // Smoothly move to target
        Vector3 start = mainCamera.transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            mainCamera.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        yield return new WaitForSeconds(2f); // Stay for 2 seconds

        // Move back to original position
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            mainCamera.transform.position = Vector3.Lerp(target, start, t);
            yield return null;
        }
    }

    // ========================= Position Logic =========================
    bool FindFreePosition(out Vector3 pos)
    {
        // First try zone A positions
        foreach (Vector3 p in GetZoneAPositions())
        {
            if (!filledPositions.Contains(p))
            {
                pos = p;
                return true;
            }
        }

        // Then try zone B
        foreach (Vector3 p in GetZoneBPositions())
        {
            if (!filledPositions.Contains(p))
            {
                pos = p;
                return true;
            }
        }

        // No position found
        pos = Vector3.zero;
        return false;
    }

    // Zone A = top row
    List<Vector3> GetZoneAPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        float[] xs = new float[] { -7.5f, -5f, -2.5f, 7.5f, 5f, 2.5f };
        foreach (float x in xs)
        {
            positions.Add(new Vector3(x, -0.9f, 0f));
        }
        return positions;
    }

    // Zone B = bottom row (fallback)
    List<Vector3> GetZoneBPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        float[] xs = new float[] { 0f, -2f, -4f, -6f, -8f, 2f, 4f, 6f, 8f };
        foreach (float x in xs)
        {
            positions.Add(new Vector3(x, -2f, -0.1f));
        }
        return positions;
    }

    // ========================= Audio =========================
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
