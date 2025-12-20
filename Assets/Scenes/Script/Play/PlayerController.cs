using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public CharacterManager characterManager;   // 현재 캐릭터 관리
    public JoystickScript joystick;             // 조이스틱 스크립트

    [Header("Interaction")]
    public InteractionZone cameraInteractionZone;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private void Start()
    {
        if (characterManager.currentCharacter != null) rb = characterManager.currentCharacter.rb;
    }

    private void FixedUpdate()
    {
        if (characterManager.currentCharacter == null) return;
        if (joystick == null) return;

        rb = characterManager.currentCharacter.rb;
        if (rb == null) return;

        Vector2 input = joystick.Direction;
        if (input != Vector2.zero) rb.velocity = input * moveSpeed;
        else rb.velocity = Vector2.zero;

        if (joystick.InteractPressed) TryInteract();
    }
    private void TryInteract()
    {
        if (cameraInteractionZone == null)
        {
            Debug.LogError("Interactionzone unconnected");
            return;
        }
        if (cameraInteractionZone.playerInZone)
        {
            Debug.Log("Camera Interacted");
        }
    }
}