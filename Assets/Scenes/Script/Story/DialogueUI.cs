using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Text characterText;
    public Text dialogueText;
    public Image characterImage; // 대화창에 표시할 캐릭터 이미지
    public GameObject panel;

    private Coroutine typingCoroutine;

    public void ShowDialogue(string character, string text, Sprite portrait = null)
    {
        panel.SetActive(true);
        characterText.text = character;
        characterImage.sprite = portrait;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
        typingCoroutine = null;
    }

    public bool IsTyping => typingCoroutine != null;
}
