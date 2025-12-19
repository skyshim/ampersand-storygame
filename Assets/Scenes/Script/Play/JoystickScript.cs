using UnityEngine;

public class JoystickScript : MonoBehaviour
{
    // 현재 방향 입력
    public Vector2 Direction { get; private set; }

    // 상호작용 버튼 상태
    public bool InteractPressed { get; private set; }

    // Joystick 오브젝트 (하위에 Up/Down/Left/Right 버튼이 있음)
    public GameObject joystick;

    // 상호작용 버튼 (Canvas 하위에 따로 배치)
    public GameObject interactButton;

    /// <summary>
    /// 조이스틱 UI 켜기
    /// </summary>
    public void ShowJoystick()
    {
        joystick.SetActive(true);
        interactButton.SetActive(true);
        Direction = Vector2.zero;
        InteractPressed = false;
    }

    /// <summary>
    /// 조이스틱 UI 끄기
    /// </summary>
    public void HideJoystick()
    {
        joystick.SetActive(false);
        interactButton.SetActive(false);
        Direction = Vector2.zero;
        InteractPressed = false;
    }

    // 방향 버튼 이벤트 (PointerDown/PointerUp에 연결)
    public void OnUpDown() => Direction = Vector2.up;
    public void OnDownDown() => Direction = Vector2.down;
    public void OnLeftDown() => Direction = Vector2.left;
    public void OnRightDown() => Direction = Vector2.right;

    public void OnButtonUp() => Direction = Vector2.zero;

    // 상호작용 버튼 이벤트
    public void OnInteractDown() => InteractPressed = true;
    public void OnInteractUp() => InteractPressed = false;

    public void Start()
    {
        HideJoystick();
    }
}