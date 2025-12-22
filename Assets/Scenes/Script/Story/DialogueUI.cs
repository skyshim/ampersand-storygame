using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum MessageType
{
    Dialogue,   // 하단 대화창
    Balloon,    // 캐릭터 말풍선
    Info        // 안내 / 설명창
}

public class DialogueUI : MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public Text characterText;
    public Text dialogueText;
    public Image characterImage;

    [Header("Balloon")]
    public GameObject balloonRoot;
    public Text balloonText;
    public RectTransform balloonTransform;
    public RectTransform bubbleImageRect;

    [Header("Info")]
    public GameObject infoPanel;
    public Text infoText;

    [Header("Settings")]
    public float typeSpeed = 0.03f;
    public Vector3 screenOffset = new Vector3(5, 5, 0);
    public AudioSource effectSource;

    private Coroutine typingCoroutine;
    private Transform balloonTarget;
    private bool followBalloon;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    public void ShowMessage(
        MessageType type,
        string text,
        string character = "",
        Sprite portrait = null,
        Transform target = null)
    {
        StopTyping();
        DisableAll();

        effectSource.Play();
        switch (type)
        {
            case MessageType.Dialogue:
                dialoguePanel.SetActive(true);
                characterText.text = character;
                characterImage.sprite = portrait;
                typingCoroutine = StartCoroutine(TypeText(dialogueText, text));
                break;

            case MessageType.Balloon:
                followBalloon = true;
                balloonTarget = target;
                balloonRoot.SetActive(true);
                typingCoroutine = StartCoroutine(TypeBalloon(text));
                break;

            case MessageType.Info:
                infoPanel.SetActive(true);
                typingCoroutine = StartCoroutine(TypeText(infoText, text));
                break;
        }
    }

    void LateUpdate()
    {
        if (!followBalloon || balloonTarget == null) return;

        Vector3 pos = mainCam.WorldToScreenPoint(balloonTarget.position);
        if (pos.z < 0) return;

        balloonTransform.position = pos + screenOffset;
    }

    IEnumerator TypeText(Text target, string text)
    {
        target.text = "";
        foreach (char c in text)
        {
            target.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        typingCoroutine = null;
    }

    IEnumerator TypeBalloon(string text)
    {
        balloonText.text = text;
        ResizeBalloon();
        balloonText.text = "";

        foreach (char c in text)
        {
            balloonText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        followBalloon = false;
        balloonTarget = null;
        typingCoroutine = null;
    }

    void ResizeBalloon()
    {
        float padding = 25f;
        float width = balloonText.preferredWidth + padding;
        bubbleImageRect.sizeDelta = new Vector2(width, bubbleImageRect.sizeDelta.y);
    }

    void StopTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = null;
    }

    void DisableAll()
    {
        dialoguePanel.SetActive(false);
        balloonRoot.SetActive(false);
        infoPanel.SetActive(false);
        followBalloon = false;
        balloonTarget = null;
    }

    public void DisableDialogue()
    {
        StopTyping();
        DisableAll();
    }

    public bool IsTyping => typingCoroutine != null;

    public bool IsShowingMessage =>
        dialoguePanel.activeSelf ||
        balloonRoot.activeSelf ||
        infoPanel.activeSelf;
}