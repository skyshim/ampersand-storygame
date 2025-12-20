using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public Camera mainCamera;

    [Header("Follow Settings")]
    public Transform followTarget;
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private bool isFollowing = false;

    private void Awake()
    {
        Instance = this;
    }

    public void FixedUpdate()
    {
        if (!isFollowing || followTarget == null) return;

        Vector3 targetPos = followTarget.position + offset;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, followSpeed * Time.deltaTime);
    }
    public void StartFollow(Transform target)
    {
        followTarget = target;
        isFollowing = true;
    }
    public void StopFollow()
    {
        isFollowing = false;
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
