using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterManager characterManager;   // 현재 캐릭터 관리
    public JoystickScript joystick;             // 조이스틱 스크립트
    public float moveSpeed = 5f;

    void Update()
    {
        if (characterManager.currentCharacter == null) return;

        Vector2 input = joystick.Direction;

        if (input == Vector2.zero) return; // 입력 없으면 멈춤

        // 이동 처리
        characterManager.currentCharacter.transform.position +=
            (Vector3)(input * moveSpeed * Time.deltaTime);

        // 상호작용 처리
        if (joystick.InteractPressed)
        {
            Debug.Log("InterAction!");
            // Interact 로직 호출
        }
    }
}