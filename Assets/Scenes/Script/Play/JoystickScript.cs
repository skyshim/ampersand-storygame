using UnityEngine;

public class JoystickScript : MonoBehaviour
{
    public RectTransform joystickBack;   // JoystickBack (Image)
    public RectTransform joystickHandle; // JoystickHandle (Image)

    private Vector2 inputVector;
    private bool isEnabled = false;

    void Start()
    {
        joystickBack.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isEnabled) return;

        // 터치 입력
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                ShowJoystick(touch.position);
            else if (touch.phase == TouchPhase.Moved)
                UpdateHandle(touch.position);
            else if (touch.phase == TouchPhase.Ended)
                HideJoystick();
        }

        // 마우스 입력
        if (Input.GetMouseButtonDown(0))
            ShowJoystick(Input.mousePosition);

        if (Input.GetMouseButton(0))
            UpdateHandle(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
            HideJoystick();
    }

    public Vector2 Direction => SnapTo8Directions(inputVector);

    private Vector2 SnapTo8Directions(Vector2 input)
    {
        if (input == Vector2.zero) return Vector2.zero;

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        float rad = snappedAngle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    public void EnableJoystick(bool enable)
    {
        isEnabled = enable;
        if (!enable) HideJoystick();
    }

    private void ShowJoystick(Vector2 screenPosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)joystickBack.parent,
            screenPosition,
            null, // Overlay 모드일 경우 null, Camera 모드면 Camera.main
            out localPoint
        );

        joystickBack.anchoredPosition = localPoint;
        joystickHandle.anchoredPosition = Vector2.zero;
        joystickBack.gameObject.SetActive(true);
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
        inputVector = Vector2.zero;
    }
}