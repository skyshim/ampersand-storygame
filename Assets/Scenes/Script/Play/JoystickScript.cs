using UnityEngine;

public class JoystickScript : MonoBehaviour
{
    // ===== 입력 상태 =====
    public Vector2 Direction { get; private set; }
    public bool InteractPressed { get; private set; }

    // ===== UI =====
    public CanvasGroup joystickCanvasGroup;
    public CanvasGroup interactButtonCanvasGroup;

    // ===== 캐릭터 =====
    public Rigidbody2D playerRb;
    public Animator playerAnimator;
    public float moveSpeed = 3f;

    // 마지막 바라본 방향
    private Vector2 lastDirection = Vector2.down;
    private Vector2 currentAnimDirection = Vector2.zero;
    private bool isWalking = false;

    // 조이스틱 활성화 상태
    private bool isJoystickEnabled = false;

    void Start()
    {
        HideJoystick();
    }

    void Update()
    {
        // 조이스틱이 비활성화 상태면 애니메이션 처리 안 함
        if (!isJoystickEnabled) return;

        HandleAnimation();
    }

    void FixedUpdate()
    {
        // 조이스틱이 비활성화 상태면 움직임 처리 안 함
        if (!isJoystickEnabled || playerRb == null) return;

        playerRb.velocity = Direction.normalized * moveSpeed;
    }

    // =============================
    // 애니메이션 처리
    // =============================
    void HandleAnimation()
    {
        if (playerAnimator == null) return;

        // 걷기 시작
        if (Direction != Vector2.zero)
        {
            lastDirection = Direction;

            // 방향이 바뀌었거나 처음 걷는 경우만 트리거
            if (!isWalking || Direction != currentAnimDirection)
            {
                isWalking = true;
                currentAnimDirection = Direction;
                PlayWalk(Direction);
            }
        }
        // 멈춤
        else
        {
            if (isWalking)
            {
                isWalking = false;
                PlayIdle(lastDirection);
            }
        }
    }

    void PlayWalk(Vector2 dir)
    {
        ResetTriggers();
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0) playerAnimator.SetTrigger("rightwalk");
            else playerAnimator.SetTrigger("leftwalk");
        }
        else
        {
            if (dir.y > 0) playerAnimator.SetTrigger("backwalk");
            else playerAnimator.SetTrigger("frontwalk");
        }
    }

    void PlayIdle(Vector2 dir)
    {
        ResetTriggers();
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0) playerAnimator.SetTrigger("right");
            else playerAnimator.SetTrigger("left");
        }
        else
        {
            if (dir.y > 0) playerAnimator.SetTrigger("back");
            else playerAnimator.SetTrigger("front");
        }
    }

    void ResetTriggers()
    {
        playerAnimator.ResetTrigger("backwalk");
        playerAnimator.ResetTrigger("frontwalk");
        playerAnimator.ResetTrigger("leftwalk");
        playerAnimator.ResetTrigger("rightwalk");
        playerAnimator.ResetTrigger("front");
        playerAnimator.ResetTrigger("back");
        playerAnimator.ResetTrigger("left");
        playerAnimator.ResetTrigger("right");
    }

    // =============================
    // UI 제어
    // =============================
    public void ShowJoystick()
    {
        Debug.Log("ShowJoystick called");
        isJoystickEnabled = true;

        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.alpha = 1f;
            joystickCanvasGroup.interactable = true;
            joystickCanvasGroup.blocksRaycasts = true;
        }

        if (interactButtonCanvasGroup != null)
        {
            interactButtonCanvasGroup.alpha = 1f;
            interactButtonCanvasGroup.interactable = true;
            interactButtonCanvasGroup.blocksRaycasts = true;
        }

        Direction = Vector2.zero;
        InteractPressed = false;

        Debug.Log("Joystick Enabled");
    }

    public void HideJoystick()
    {
        Debug.Log("HideJoystick called");
        isJoystickEnabled = false;

        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.alpha = 0f;
            joystickCanvasGroup.interactable = false;
            joystickCanvasGroup.blocksRaycasts = false;
        }

        if (interactButtonCanvasGroup != null)
        {
            interactButtonCanvasGroup.alpha = 0f;
            interactButtonCanvasGroup.interactable = false;
            interactButtonCanvasGroup.blocksRaycasts = false;
        }

        Direction = Vector2.zero;
        InteractPressed = false;

        // 플레이어 멈추기
        if (playerRb != null)
            playerRb.velocity = Vector2.zero;

        Debug.Log("Joystick Disabled");
    }

    // =============================
    // 버튼 이벤트 (UI)
    // =============================
    public void OnUpDown() => Direction = Vector2.up;
    public void OnDownDown() => Direction = Vector2.down;
    public void OnLeftDown() => Direction = Vector2.left;
    public void OnRightDown() => Direction = Vector2.right;
    public void OnButtonUp() => Direction = Vector2.zero;
    public void OnInteractDown() => InteractPressed = true;
    public void OnInteractUp() => InteractPressed = false;
}