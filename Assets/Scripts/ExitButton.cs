using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";  // Set your menu scene name

    public void OnExitButtonPressed()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == mainMenuSceneName)
        {
            // Exit the game
            Application.Quit();

#if UNITY_EDITOR
            // If running in the editor
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            // Go back to Main Menu
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
