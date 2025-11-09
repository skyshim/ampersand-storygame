using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public Camera mainCamera;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator MoveCamera(Vector3 targetPos, float duration, float targetSize)
    {
        Vector3 startPos = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.orthographicSize = targetSize;
    }
}
