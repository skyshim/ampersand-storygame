using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NextCondition
{
    public enum ConditionType
    {
        Auto,           // 이벤트 끝나면 자동 진행
        PlayerAction,   // 플레이어 목표 달성 시
        Trigger,        // 코드/스크립트 호출
    }

    public ConditionType type;
    public string parameter; // 예: "keyItemCollected"
}