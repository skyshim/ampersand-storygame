using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryAction
{
    public enum ActionType { Dialogue, Move, Animate, CameraMove, BackgroundChange, Wait }
    public ActionType type;

    // Dialogue
    public string characterName;
    [TextArea] public string dialogueText;
    public bool isBalloon;

    // Move
    public Vector3 targetPosition;
    public float moveDuration = 1f;

    // Animate
    public string animationTrigger;

    // CameraMove
    public Vector3 cameraTargetPosition = new Vector3(0f, 0f, -10f);
    public float cameraMoveDuration = 1f;
    public float cameraTargetSize = 5f; // ¡‹¿Œ/æ∆øÙ

    // BackgroundChange
    public Sprite newBackground; // πŸ≤‹ πË∞Ê ¿ÃπÃ¡ˆ

    // Wait
    public float waitDuration = 1f;
}
