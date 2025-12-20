using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Text characterText;
    public Text dialogueText;
    public Image characterImage;
    public GameObject panel;

    public GameObject balloonRoot;
    public Text balloonText;
    public RectTransform balloonTransform;
    public RectTransform bubbleImageRect;

    public Vector3 screenOffset = new Vector3(5f, 5f, 0);

    private Coroutine typingCoroutine;
    private Transform balloonTarget;

    private bool isBalloonActive = false;

    private Camera mainCam;
    void Awake()
    {
        mainCam = Camera.main;
    }

    public void ShowDialogue(
            string character,
            string text,
            bool isBalloon,
            Sprite portrait = null,
            Transform target = null
        )
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (isBalloon)
        {
            // 말풍선 모드
            isBalloonActive = true;
            balloonTarget = target;

            panel.SetActive(false);
            balloonRoot.SetActive(true);

            typingCoroutine = StartCoroutine(TypeBalloon(text));
        }
        else
        {
            // 일반 대화창
            DisableBalloon();

            panel.SetActive(true);
            characterText.text = character;
            characterImage.sprite = portrait;

            typingCoroutine = StartCoroutine(TypeText(text));
        }
    }

    void LateUpdate()
    {
        // PlayerControl 상태에서도 돌지 않도록 차단
        if (!isBalloonActive || balloonTarget == null || mainCam == null)
            return;

        Vector3 screenPos = mainCam.WorldToScreenPoint(balloonTarget.position);

        if (screenPos.z < 0)
        {
            balloonRoot.SetActive(false);
            return;
        }

        balloonRoot.SetActive(true);
        balloonTransform.position = screenPos + screenOffset;
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

    private IEnumerator TypeBalloon(string text)
    {
        // 1. 전체 텍스트로 말풍선 크기 계산
        balloonText.text = text;
        ResizeBalloon(balloonText);

        // 2. 다시 비우고 타이핑 시작
        balloonText.text = "";

        foreach (char c in text)
        {
            balloonText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        typingCoroutine = null;
    }

    void ResizeBalloon(Text text)
    {
        float padding = 25f;

        float width = text.preferredWidth + padding;

        // Image 자체를 늘린다 (9-Slice 적용)
        bubbleImageRect.sizeDelta =
            new Vector2(width, bubbleImageRect.sizeDelta.y);

        // Text는 padding 고려해서 약간 작게
        balloonText.rectTransform.sizeDelta =
            new Vector2(width - padding, balloonText.rectTransform.sizeDelta.y);
    }

    public void DisableDialogue()
    {
        DisableBalloon();
        panel.SetActive(false);
    }

    void DisableBalloon()
    {
        isBalloonActive = false;
        balloonTarget = null;
        balloonRoot.SetActive(false);
    }

    public bool IsTyping => typingCoroutine != null;
}
