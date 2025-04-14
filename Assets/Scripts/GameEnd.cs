using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For scene management

public class GameEnd : MonoBehaviour
{
    // UI Elements
    public Text endGameText;  // Text for the popup
    public Button exitButton;            // Button to go to the main menu
    public GameObject blurPanel;         // The overlay panel (with blur effect)

    void Start()
    {
        // Initially hide the game end popup and blur
        endGameText.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);  // Hide the button initially
        blurPanel.SetActive(false);  // Hide the blur overlay initially
    }

    // Call this method when the game is finished (e.g., when all employees are spawned)
    public void EndGame()
    {
        ShowEndGamePopup();
    }

    // Show the popup, display the text, and activate the blur
    void ShowEndGamePopup()
    {
        // Show the blur overlay
        blurPanel.SetActive(true);

        // Show the end game popup
        endGameText.text = "Game Over!";
        endGameText.gameObject.SetActive(true); // Show the text

        // Show the exit button
        exitButton.gameObject.SetActive(true);
    }

    // Go to the main menu scene
    public void GoToMainMenu()
    {
        // Assuming the main menu scene is called "MainMenu"
        SceneManager.LoadScene("MainMenu");
    }
}
