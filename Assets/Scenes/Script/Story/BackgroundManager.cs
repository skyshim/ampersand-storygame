using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;

    public SpriteRenderer backgroundImage;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeBackground(Sprite newBg)
    {
        if (backgroundImage != null && newBg != null)
            backgroundImage.sprite = newBg;
    }
}