using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class DialogueUI : MonoBehaviour
{
    public Text characterText;
    public Text dialogueText;
    public Image characterImage; // 대화창에 표시할 캐릭터 이미지
    public GameObject panel;

    public GameObject balloonRoot;    // 말풍선 전체
    public Text balloonText;
    public SpriteRenderer balloonRenderer;

    private Coroutine typingCoroutine;

    public void ShowDialogue(string character, string text, bool isBalloon, Sprite portrait = null, Transform target = null)
    {
        if (isBalloon)
        {
            panel.SetActive(false);
            balloonRoot.SetActive(true);

            if (target != null)
                balloonRoot.transform.position = target.position;

            typingCoroutine = StartCoroutine(TypeBalloon(text));
        }

        else {
            panel.SetActive(true);
            characterText.text = character;
            characterImage.sprite = portrait;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(text));
        }
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
    IEnumerator TypeBalloon(string text)
    {
        balloonText.text = "";

        foreach (char c in text)
        {
            balloonText.text += c;
            ResizeBalloon();
            yield return new WaitForSeconds(0.03f);
        }

        typingCoroutine = null;
    }

    public bool IsTyping => typingCoroutine != null;

    void ResizeBalloon()
    {
        float textWidth = balloonText.preferredWidth;
        float worldWidth = textWidth / 100f;

        float finalWidth = Mathf.Max(0.3f, worldWidth + 1.2f);

        Vector2 size = balloonRenderer.size;
        size.x = finalWidth;
        balloonRenderer.size = size;
    }
}
