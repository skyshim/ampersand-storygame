using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryAction
{
    public enum ActionType { Dialogue, Move, Animate, CameraMove, BackgroundChange, Wait, SetCharacter, SetSpriteVisibility, PlaySound }
    public ActionType type;

    // Dialogue
    public string characterName;
    [TextArea] public string dialogueText;
    public MessageType messageType;

    // Move
    public Vector3 targetPosition;
    public float moveDuration = 1f;

    // Animate
    public string animationTrigger;

    // CameraMove
    public Vector3 cameraTargetPosition = new Vector3(0f, 0f, -10f);
    public float cameraMoveDuration = 1f;
    public float cameraTargetSize = 5f; // 줌인/아웃

    // BackgroundChange
    public Sprite newBackground; // 바꿀 배경 이미지

    // Wait
    public float waitDuration = 1f;

    // SetCharacter
    public string playingCharacterName;

    public string targetObjectName;      // 대상 오브젝트 이름
    public float targetAlpha;       // true: 보이게, false: 안 보이게

    public AudioClip soundSource;
}
