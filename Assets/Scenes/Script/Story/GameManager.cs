using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public JoystickScript joystick;
    public StoryController storyController;

    [Header("현재 진행 상태")]
    public int currentSceneIndex = 0; // 저장/로드용
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
        isStoryMode = false;

        if (joystick != null)
        {
            joystick.ShowJoystick();
            Debug.Log("Joystick Enabled by GameManager");
        }
        else
            Debug.Log("Jotstick already Enabled");
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
        currentSceneIndex = nextSceneIndex;
        SaveProgress();

        if (isStoryMode)
        {
            StartStory();
        }
    }

    public void OnSceneTypeChanged(SceneType type)
    {
        Debug.Log("OnSceneTypeChanged called with: " + type);
        JoystickScript joystick = FindObjectOfType<JoystickScript>();
        if (joystick == null) return;

        if (type == SceneType.PlayerControl)
        {
            Debug.Log("SceneType PlayerControl detected");
            // 아직 대화가 남아있을 수 있으므로 여기서는 바로 켜지지 않음
        }
        else
        {
            joystick.HideJoystick();
            Debug.Log("Joystick Disabled (Non-PlayerControl Scene)");
        }
    }

    public void OnDialogueFinished()
    {
        // 대화까지 끝났을 때 진짜 플레이어 모드 시작
        Debug.Log("OnDialogueFinished");
        StartPlayerMode();
        Input.ResetInputAxes();

        StartPlayerMode();
    }
}