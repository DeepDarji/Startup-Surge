using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;

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
    private float currentCash = 9999999f;
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

    // Audio
    public AudioSource audioSource;
    public AudioClip earnSound;
    public AudioClip upgradeSound;
    public AudioClip hireSound;
    public AudioClip errorSound;

    private List<Vector3> occupiedPositions = new List<Vector3>();

    public GameEnd gameEndManager;

    // Function to generate Zone A positions dynamically
    List<Vector3> GenerateZoneAPositions()
    {
        List<Vector3> zoneAPositions = new List<Vector3>();
        float[] xValues = new float[] { -7.5f, -5f, -2.5f, 7.5f, 5f, 2.5f };
        float y = -0.9f;
        float z = 0f;

        foreach (float x in xValues)
        {
            zoneAPositions.Add(new Vector3(x, y, z));
        }

        return zoneAPositions;
    }

    // Function to generate Zone B positions dynamically
    List<Vector3> GenerateZoneBPositions()
    {
        List<Vector3> zoneBPositions = new List<Vector3>();
        float[] xValues = new float[] { 0f, -2f, -4f, -6f, -8f, 2f, 4f, 6f, 8f };
        float y = -2f;
        float z = -0.1f;

        foreach (float x in xValues)
        {
            zoneBPositions.Add(new Vector3(x, y, z));
        }

        return zoneBPositions;
    }

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
        PlaySound(earnSound);
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
            PlaySound(upgradeSound);
            UpdateUI();
        }
        else
        {
            ShowPopup($"Low Money! Need ${upgradeCost}");
            PlaySound(errorSound);
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

                GameObject prefab = null; // Ensure prefab is initialized

                // Determine which prefab to use based on position
                if (spawnPos.z == 0f) // Zone A
                {
                    if (spawnPos.x == -7.5f || spawnPos.x == -5f || spawnPos.x == -2.5f)
                    {
                        prefab = employeePrefabs[2]; // char3
                    }
                    else
                    {
                        prefab = employeePrefabs[4]; // char5
                    }
                }
                else if (spawnPos.z == -0.1f) // Zone B
                {
                    // In Zone B, ensure only Char1 (0), Char2 (1), or Char4 (3) spawn
                    int randIndex = Random.Range(0, 3); // Randomly choose between 0, 1, or 3 (Char1, Char2, Char4)

                    // Map the random index to the correct character prefab
                    switch (randIndex)
                    {
                        case 0:
                            prefab = employeePrefabs[0]; // Char1
                            break;
                        case 1:
                            prefab = employeePrefabs[1]; // Char2
                            break;
                        case 2:
                            prefab = employeePrefabs[3]; // Char4
                            break;
                    }
                }

                // Ensure prefab has been assigned before instantiation
                if (prefab != null)
                {
                    // Instantiate the employee at the found position
                    Instantiate(prefab, spawnPos, Quaternion.identity, employeeParent);
                    occupiedPositions.Add(spawnPos);

                    ShowPopup($"Employee Hired! Total: {employeeCount}");
                    MoveCameraTo(spawnPos.x);
                    PlaySound(hireSound);
                    UpdateUI();
                }
                else
                {
                    ShowPopup("Error: Invalid employee prefab.");
                    PlaySound(errorSound);
                }
            }
            else
            {
                ShowPopup("No space left to hire!");
                PlaySound(errorSound);
            }
        }
        else
        {
            PlaySound(errorSound);
            ShowPopup($"Low Money! Need ${employeeCost}");
        }
        if (employeeCount >= 15)  // Replace with your condition
        {
            gameEndManager.EndGame();  // Call the EndGame function in the new script
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
        List<Vector3> zoneAPositions = GenerateZoneAPositions(); // Generate Zone A positions
        List<Vector3> zoneBPositions = GenerateZoneBPositions(); // Generate Zone B positions

        // Try Zone A first
        foreach (Vector3 p in zoneAPositions)
        {
            if (!occupiedPositions.Contains(p))
            {
                pos = p;
                return true;
            }
        }

        // If Zone A is filled, check Zone B
        foreach (Vector3 p in zoneBPositions)
        {
            if (!occupiedPositions.Contains(p))
            {
                pos = p;
                return true;
            }
        }

        // If no space found
        pos = Vector3.zero;
        return false;
    }



    /*bool IsOccupied(Vector3 position)
        {
            float minSpacing = 1.5f;
            Collider[] colliders = Physics.OverlapSphere(position, minSpacing);
            foreach (Collider c in colliders)
            {
                if (c.CompareTag("Employee")) return true;
            }
            return false;
        }*/

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

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

}
