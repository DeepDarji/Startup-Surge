using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    public string sceneToLoad = "Game";      // Set your gameplay scene name
    public AudioClip clickSound;                  // Assign click sound
    public AudioSource audioSource;               // Assign an AudioSource (can be UI Audio Source)

    public void OnStartButtonClicked()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Load the scene after a short delay to allow the sound to play
        Invoke(nameof(LoadScene), 0.2f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
