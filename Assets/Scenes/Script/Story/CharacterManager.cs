using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string characterName;
    public Transform transform;
    public Animator animator;
    public Sprite portrait;
}

public class CharacterManager : MonoBehaviour
{
    public List<Character> characters;
    public Character currentCharacter;

    public Character GetCharacter(string name)
    {
        return characters.Find(c => c.characterName == name);
    }

    public void SetCurrentCharacter(string name)
    {
        currentCharacter = GetCharacter(name);

        if (currentCharacter == null)
            Debug.Log("캐릭터 못 찾음 :" + name);
        else
            Debug.Log("캐릭터 지정함 :" + name);
    }

    public IEnumerator MoveCharacter(string name, Vector3 target, float duration)
    {
        var c = GetCharacter(name);
        if (c == null)
        {
            Debug.LogWarning("캐릭터 못찾음: " + name);
            yield break;
        }

        Vector3 start = c.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            c.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        c.transform.position = target;
    }
    public void PlayAnimation(string name, string trigger)
    {
        var c = GetCharacter(name);
        if (c == null) return;
        c.animator.SetTrigger(trigger);
    }
}
