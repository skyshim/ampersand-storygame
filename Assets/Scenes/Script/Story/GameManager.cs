using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public JoystickScript joystick;
    public StoryController storyController;

    [Header("ÇöÀç ÁøÇà »óÅÂ")]
    public int currentSceneIndex = 0;
    public bool isStoryMode = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartStory();
    }

    public void StartStory()
    {
        isStoryMode = true;
        storyController.StartScene(currentSceneIndex);
    }

    public void StartPlayerMode()
    {
        Debug.Log("StartPlayerMode called");
        isStoryMode = false;

        if (joystick != null)
        {
            joystick.ShowJoystick();
            Debug.Log("Joystick Enabled by GameManager");
        }
        else
        {
            Debug.LogWarning("Joystick is null!");
        }
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentSceneIndex", currentSceneIndex);
    }

    public void LoadProgress()
    {
        currentSceneIndex = PlayerPrefs.GetInt("CurrentSceneIndex", 0);
    }

    public void OnSceneComplete(int nextSceneIndex)
    {
        Debug.Log($"OnSceneComplete called: {nextSceneIndex}");
        currentSceneIndex = nextSceneIndex;
        SaveProgress();
    }

    public void OnSceneTypeChanged(SceneType type)
    {
        Debug.Log("OnSceneTypeChanged called with: " + type);

        // PlayerControlÀÌ ¾Æ´Ñ ¾À¿¡¼­¸¸ Á¶ÀÌ½ºÆ½ ¼û±è
        if (type != SceneType.PlayerControl)
        {
            if (joystick != null)
            {
                joystick.HideJoystick();
                Debug.Log("Joystick Disabled (Non-PlayerControl Scene)");
            }
        }
        else
        {
            Debug.Log("PlayerControl Scene - Joystick will be enabled after dialogue");
        }
    }

    public void OnDialogueFinished()
    {
        Debug.Log("OnDialogueFinished called");
        StartPlayerMode();
        Input.ResetInputAxes();

        var current = storyController.characterManager.currentCharacter;
        if (current != null)
        {
            CameraManager.Instance.StartFollow(current.transform);
            Debug.Log($"Camera following: {current.characterName}");
        }
        else
        {
            Debug.LogWarning("currentCharacter is null!");
        }
    }
}