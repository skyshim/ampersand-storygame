using UnityEngine;
using UnityEngine.UI;

public class JoystickScript : MonoBehaviour
{
    public RectTransform joystickBack;
    public RectTransform joystickHandle;
    public Canvas canvas;

    private Vector2 inputVector;
    private bool isEnabled;

    void Start() => EnableJoystick(false);

    void Update()
    {
        if (!isEnabled) return;

        if (Input.GetMouseButtonDown(0)) ShowJoystick(Input.mousePosition);
        else if (Input.GetMouseButton(0)) UpdateHandle(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0)) HideJoystick();
    }

    public void EnableJoystick(bool enable)
    {
        isEnabled = enable;
        if (!enable) HideJoystick();
    }

    private void ShowJoystick(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out var localPoint
        );

        joystickBack.anchoredPosition = localPoint;
        joystickHandle.localPosition = Vector3.zero;

        joystickBack.gameObject.SetActive(true);
        joystickHandle.gameObject.SetActive(true);
    }

    private void UpdateHandle(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBack, screenPosition, null, out var localPoint
        );

        var clamped = Vector2.ClampMagnitude(localPoint, joystickBack.sizeDelta.x / 2f);
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