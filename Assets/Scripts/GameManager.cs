using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // UI Elements
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI passiveIncomeText;
    public TextMeshProUGUI employeeCountText;
    public Text earnButtonText;
    public Text upgradeButtonText;
    public Text hireButtonText;
    public TextMeshProUGUI popupText;

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

    // Employee Prefabs
    public GameObject[] employeePrefabs;
    public Transform employeeParent;

    // Camera
    public Camera mainCamera;
    private Vector3 cameraMinPos = new Vector3(-6f, 0f, -10f);
    private Vector3 cameraMaxPos = new Vector3(6f, 0f, -10f);

    private List<Vector3> occupiedPositions = new List<Vector3>();

    void Start()
    {
        earnButton.onClick.AddListener(OnEarnClicked);
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        hireButton.onClick.AddListener(OnHireClicked);

        InvokeRepeating("AddPassiveIncome", 1f, 1f);
        popupText.gameObject.SetActive(false);
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
            incomePerClick++;
            upgradeLevel++;
            upgradeCost = Mathf.RoundToInt(upgradeCost * 1.7f);
            ShowPopup("Upgrade successful!");
            UpdateUI();
        }
        else
        {
            ShowPopup($"Low Money! Need ${upgradeCost}");
        }
    }

    void OnHireClicked()
    {
        if (currentCash >= employeeCost)
        {
            Vector3 spawnPos;
            if (FindSpawnPosition(out spawnPos))
            {
                currentCash -= employeeCost;
                employeeCount++;
                passiveIncomePerSec += 1f;
                employeeCost = Mathf.RoundToInt(employeeCost * 1.5f);

                GameObject prefab = employeePrefabs[Random.Range(0, employeePrefabs.Length)];
                Instantiate(prefab, spawnPos, Quaternion.identity, employeeParent);
                occupiedPositions.Add(spawnPos);

                ShowPopup($"Employee Hired! Total: {employeeCount}");
                MoveCameraTo(spawnPos.x);
                UpdateUI();
            }
            else
            {
                ShowPopup("No space left to hire!");
            }
        }
        else
        {
            ShowPopup($"Low Money! Need ${employeeCost}");
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

    void ShowPopup(string message)
    {
        popupText.text = message;
        popupText.gameObject.SetActive(true);
        popupText.transform.localScale = Vector3.one * 0.8f;
        popupText.alpha = 1f;

        StopCoroutine("AnimatePopup"); // Stop previous one if running
        StartCoroutine(AnimatePopup());
    }

    void HidePopup()
    {
        popupText.gameObject.SetActive(false);
    }

    IEnumerator AnimatePopup()
    {
        float duration = 2f;
        float elapsed = 0f;

        Vector3 startScale = Vector3.one * 0.8f;
        Vector3 endScale = Vector3.one;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            popupText.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            popupText.alpha = Mathf.Lerp(1f, 0f, t); // Fade out

            yield return null;
        }

        popupText.gameObject.SetActive(false);
    }

    void MoveCameraTo(float targetX)
    {
        Vector3 target = new Vector3(Mathf.Clamp(targetX, cameraMinPos.x, cameraMaxPos.x), 0f, -10f);
        StopAllCoroutines(); // Stop if a camera move is already happening
        StartCoroutine(MoveCameraRoutine(target));
    }

    bool FindSpawnPosition(out Vector3 pos)
    {
        List<Vector3> potentialSpawns = new List<Vector3>();

        // Zone A: Left side (-7.5 to -2)
        for (float x = -7.5f; x <= -2f; x += 2f)
        {
            Vector3 p = new Vector3(x, -0.9f, 0f);
            if (!IsOccupied(p)) potentialSpawns.Add(p);
        }

        // Zone B: Right side (2 to 7.5)
        for (float x = 2f; x <= 7.5f; x += 2f)
        {
            Vector3 p = new Vector3(x, -0.9f, 0f);
            if (!IsOccupied(p)) potentialSpawns.Add(p);
        }

        // Zone C: fallback (-7.5 to 7.5, z = -0.1)
        if (potentialSpawns.Count == 0)
        {
            for (float x = -7.5f; x <= 7.5f; x += 2f)
            {
                Vector3 p = new Vector3(x, -0.9f, -0.1f);
                if (!IsOccupied(p)) potentialSpawns.Add(p);
            }
        }

        if (potentialSpawns.Count > 0)
        {
            pos = potentialSpawns[Random.Range(0, potentialSpawns.Count)];
            return true;
        }

        pos = Vector3.zero;
        return false;
    }

    bool IsOccupied(Vector3 checkPos)
    {
        foreach (var pos in occupiedPositions)
        {
            if (Mathf.Abs(pos.x - checkPos.x) < 2f && Mathf.Abs(pos.z - checkPos.z) < 0.2f)
                return true;
        }
        return false;
    }

    IEnumerator MoveCameraRoutine(Vector3 targetPos)
    {
        Vector3 originalPos = mainCamera.transform.position;
        float t = 0f;

        // Move to target
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; // Speed of camera move
            mainCamera.transform.position = Vector3.Lerp(originalPos, targetPos, t);
            yield return null;
        }

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        t = 0f;
        // Move back to original
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            mainCamera.transform.position = Vector3.Lerp(targetPos, originalPos, t);
            yield return null;
        }
    }

}
