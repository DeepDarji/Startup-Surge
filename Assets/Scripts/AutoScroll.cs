using UnityEngine;

public class AutoScroll : MonoBehaviour
{
    public Transform cameraTransform;  // Assign main camera here
    public float scrollSpeed = 5f;

    public void ScrollTo(Vector3 targetPosition)
    {
        StartCoroutine(SmoothScroll(targetPosition));
    }

    private System.Collections.IEnumerator SmoothScroll(Vector3 target)
    {
        Vector3 startPos = cameraTransform.position;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            cameraTransform.position = Vector3.Lerp(startPos, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = target;
    }
}
