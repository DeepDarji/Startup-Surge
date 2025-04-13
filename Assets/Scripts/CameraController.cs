using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 0.1f;
    private Vector3 dragOrigin;

    public float minX = -6f;
    public float maxX = 6f;

    private bool isCameraLocked = false;
    private Vector3 originalPosition;

    void Update()
    {
        if (isCameraLocked) return;

        // Dragging with mouse
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = transform.position + new Vector3(difference.x, 0, 0);

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = 0f;
            newPos.z = -10f;

            transform.position = newPos;
        }
    }

    public void FocusOn(Vector3 targetPosition)
    {
        if (!isCameraLocked)
        {
            StartCoroutine(FocusRoutine(targetPosition));
        }
    }

    IEnumerator FocusRoutine(Vector3 target)
    {
        isCameraLocked = true;
        originalPosition = transform.position;

        Vector3 focusPosition = new Vector3(Mathf.Clamp(target.x, minX, maxX), 0f, -10f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; // smooth speed
            transform.position = Vector3.Lerp(originalPosition, focusPosition, t);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(focusPosition, originalPosition, t);
            yield return null;
        }

        isCameraLocked = false;
    }
}
