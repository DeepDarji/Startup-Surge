using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour
{
    [Header("UI Elements to Animate")]
    public RectTransform[] uiElements; // Assign 1 or more UI objects (buttons/images)

    [Header("Animation Settings")]
    public float popupDuration = 0.3f;
    public Vector3 finalScale = new Vector3(3f, 3f, 1f);

    [Header("Optional Delay Between Elements")]
    public bool useStaggeredPopup = false;
    public float delayBetween = 0.1f; // Delay between each popup (if enabled)

    private float[] timers;
    private bool allPopped = false;

    void Start()
    {
        timers = new float[uiElements.Length];

        for (int i = 0; i < uiElements.Length; i++)
        {
            if (uiElements[i] != null)
                uiElements[i].localScale = Vector3.zero; // Start small
        }
    }

    void Update()
    {
        if (allPopped) return;

        bool allDone = true;

        for (int i = 0; i < uiElements.Length; i++)
        {
            if (uiElements[i] == null) continue;

            float delay = useStaggeredPopup ? delayBetween * i : 0f;

            if (timers[i] < popupDuration + delay)
            {
                timers[i] += Time.deltaTime;

                float progress = Mathf.Clamp01((timers[i] - delay) / popupDuration);
                uiElements[i].localScale = Vector3.Lerp(Vector3.zero, finalScale, progress);

                if (progress < 1f)
                    allDone = false;
            }
        }

        if (allDone)
            allPopped = true;
    }
}