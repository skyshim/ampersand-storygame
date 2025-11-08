using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryScene", menuName = "Story/StoryScene")]
public class StoryScene : ScriptableObject
{
    public string sceneName;
    public SceneType sceneType;
    public List<StoryEvent> events;          // 장면 안에서 실행할 이벤트
    public NextCondition nextCondition;      // 다음 Scene으로 넘어가는 조건
}

public enum SceneType
{
    Dialogue,
    PlayerControl,
    Cutscene
}