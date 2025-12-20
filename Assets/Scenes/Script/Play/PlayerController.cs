using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public CharacterManager characterManager;   // 현재 캐릭터 관리
    public JoystickScript joystick;             // 조이스틱 스크립트
    public float moveSpeed = 5f;

    private void FixedUpdate()
    {
        if (characterManager.currentCharacter == null) return;

        var character = characterManager.currentCharacter;
        if (character.rb == null) return;

        Vector2 input = joystick.Direction;
        if (input == Vector2.zero)
        {
            character.rb.velocity = Vector2.zero;
            return;
        }

        Vector2 targetPos = character.rb.position + input.normalized * moveSpeed * Time.fixedDeltaTime;
        character.rb.MovePosition(targetPos);
    }
}