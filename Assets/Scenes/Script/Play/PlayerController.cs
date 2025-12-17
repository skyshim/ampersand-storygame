using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterManager characterManager;   // 현재 캐릭터 관리
    public JoystickScript joystick;             // 아까 만든 조이스틱 스크립트
    public float moveSpeed = 5f;

    void Update()
    {
        if (characterManager.currentCharacter == null) return;

        // 조이스틱 입력값 가져오기
        Vector2 input = joystick.Direction;

        if (input == Vector2.zero)
            return;

        // 4방향으로 제한 (x, y 중 큰 쪽만 살림)
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            input = new Vector2(Mathf.Sign(input.x), 0);
        else
            input = new Vector2(0, Mathf.Sign(input.y));

        // 이동
        characterManager.currentCharacter.transform.position +=
            (Vector3)(input * moveSpeed * Time.deltaTime);

        Debug.Log("Joystick Direction: " + joystick.Direction);
    }
}