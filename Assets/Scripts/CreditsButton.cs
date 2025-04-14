using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsButton : MonoBehaviour
{
    public string creditsSceneName = "CreditsScene";  // Make sure this matches your scene name
    public AudioClip clickSound;
    public AudioSource audioSource;

    public void OnCreditsButtonClicked()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        Invoke(nameof(LoadCreditsScene), 0.2f);
    }

    void LoadCreditsScene()
    {
        SceneManager.LoadScene(creditsSceneName);
    }
}
