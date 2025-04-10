using System.Collections;
using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public Animator popupAnimator; // Optional (only if you're using an animation clip)
    public AudioSource popupAudio; // Optional (only if you want sound)

    private Coroutine popupRoutine;

    public void ShowPopup(string message)
    {
        popupText.text = message;
        popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, 1f);

        // Play animation (optional)
        if (popupAnimator != null && !popupAnimator.GetCurrentAnimatorStateInfo(0).IsName("PopupBounce"))
        {
            popupAnimator.Play("PopupBounce", -1, 0f);
        }

        // Play sound (optional)
        if (popupAudio != null)
        {
            popupAudio.Play();
        }

        // Handle coroutine timing
        if (popupRoutine != null)
            StopCoroutine(popupRoutine);
        popupRoutine = StartCoroutine(HidePopupAfterDelay());
    }

    IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(2f); // How long the popup stays visible

        // Smooth fade out
        float fadeDuration = 0.5f;
        float t = 0f;
        Color originalColor = popupText.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            popupText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        popupText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Fully invisible
    }
}
