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
    public Rigidbody2D rb;
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
        if (currentCharacter != null && currentCharacter.rb != null)
            currentCharacter.rb.velocity = Vector2.zero;

        currentCharacter = GetCharacter(name);

        if (currentCharacter == null)
            Debug.Log("캐릭터 못 찾음 :" + name);
        else
        {
            Debug.Log("캐릭터 지정함 :" + name);
            CameraManager.Instance.StartFollow(currentCharacter.transform);
        }
    }

    public IEnumerator MoveCharacter(string name, Vector3 target, float duration)
    {
        var c = GetCharacter(name);
        if (c == null || c.rb == null)
        {
            Debug.LogWarning("캐릭터 못찾음: " + name);
            yield break;
        }

        Vector2 start = c.rb.position;
        Vector2 end = target;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector2 pos = Vector2.Lerp(start, end, t);
            c.rb.MovePosition(pos);
            yield return null;
        }

        c.rb.MovePosition(end);
    }
    public void PlayAnimation(string name, string trigger)
    {
        var c = GetCharacter(name);
        if (c == null || c.animator == null) return;
        c.animator.SetTrigger(trigger);
    }
}
