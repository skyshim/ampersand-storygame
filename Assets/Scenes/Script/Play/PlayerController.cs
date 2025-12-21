using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterManager characterManager;
    public JoystickScript joystick;
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private bool lastInteractState = false;

    private void Start()
    {
        if (characterManager.currentCharacter != null)
            rb = characterManager.currentCharacter.rb;
    }

    private void FixedUpdate()
    {
        if (characterManager.currentCharacter == null) return;
        if (joystick == null) return;

        rb = characterManager.currentCharacter.rb;
        if (rb == null) return;

        // 이동 처리
        Vector2 input = joystick.Direction;
        if (input != Vector2.zero)
            rb.velocity = input * moveSpeed;
        else
            rb.velocity = Vector2.zero;

        // 상호작용 처리 (버튼을 눌렀을 때만 한 번 호출)
        if (joystick.InteractPressed && !lastInteractState)
        {
            TryInteract();
        }
        lastInteractState = joystick.InteractPressed;
    }

    private void TryInteract()
    {
        Debug.Log("TryInteract called");

        if (InteractionManager.Instance != null)
        {
            InteractionManager.Instance.TryInteract();
        }
        else
        {
            Debug.LogError("InteractionManager.Instance is null!");
        }
    }
}