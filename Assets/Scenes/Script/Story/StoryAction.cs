using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryAction
{
    public enum ActionType { Dialogue, Move, Animate }
    public ActionType type;

    // Dialogue
    public string characterName;
    [TextArea] public string dialogueText;

    // Move
    public Vector3 targetPosition;
    public float moveDuration = 1f;

    // Animate
    public string animationTrigger;
}
