using UnityEngine;
using UnityEngine.UI;

public class JoystickScript : MonoBehaviour
{
    public RectTransform joystickBack;
    public RectTransform joystickHandle;
    public Canvas canvas;

    private Vector2 inputVector;
    private bool isEnabled = false;

    void Start()
    {
        // 오브젝트는 Active 상태로 두고, UI만 숨김
        joystickBack.gameObject.SetActive(false);
        joystickHandle.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isEnabled) return;

        if (Input.GetMouseButtonDown(0))
            ShowJoystick(Input.mousePosition);

        if (Input.GetMouseButton(0))
            UpdateHandle(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
            HideJoystick();
    }

    public void EnableJoystick(bool enable)
    {
        Debug.Log("EnableJoystick called: " + enable);
        isEnabled = enable;
        if (!enable) HideJoystick();
    }

    private void ShowJoystick(Vector2 screenPosition)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        // 백 위치 설정
        joystickBack.anchoredPosition = localPoint;

        // 핸들 기준 중앙 고정
        joystickHandle.anchorMin = joystickHandle.anchorMax = new Vector2(0.5f, 0.5f);
        joystickHandle.pivot = new Vector2(0.5f, 0.5f);
        joystickHandle.localPosition = Vector3.zero;

        joystickBack.gameObject.SetActive(true);
        joystickHandle.gameObject.SetActive(true);

        Debug.Log("Joystick spawned at: " + localPoint);
    }

    private void UpdateHandle(Vector2 screenPosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBack,
            screenPosition,
            null,
            out localPoint
        );

        Vector2 clamped = Vector2.ClampMagnitude(localPoint, joystickBack.sizeDelta.x / 2f);
        joystickHandle.anchoredPosition = clamped;
        inputVector = clamped.normalized;
    }

    private void HideJoystick()
    {
        joystickBack.gameObject.SetActive(false);
        joystickHandle.gameObject.SetActive(false);
        inputVector = Vector2.zero;
    }

    public Vector2 Direction => inputVector;
}