using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryEvent
{
    public string description; // 에디터용 이름
    public List<StoryAction> actions;
}
