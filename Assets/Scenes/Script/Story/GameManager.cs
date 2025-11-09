using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    /// <summary>
    /// 스토리 모드 시작
    /// </summary>
    public void StartStory()
    {
        isStoryMode = true;
        storyController.StartScene(currentSceneIndex);
    }

    /// <summary>
    /// 플레이어 모드 시작
    /// </summary>
    public void StartPlayerMode()
    {
        isStoryMode = false;
        // 플레이어 조작 활성화
    }

    /// <summary>
    /// 스토리 진행 상태 저장
    /// </summary>
    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentSceneIndex", currentSceneIndex);
    }

    /// <summary>
    /// 스토리 진행 상태 불러오기
    /// </summary>
    public void LoadProgress()
    {
        currentSceneIndex = PlayerPrefs.GetInt("CurrentSceneIndex", 0);
    }

    /// <summary>
    /// 스토리 Scene이 끝났을 때 호출
    /// </summary>
    public void OnSceneComplete(int nextSceneIndex)
    {
        currentSceneIndex = nextSceneIndex;
        SaveProgress();
        

        if (isStoryMode)
        {
            StartStory();
        }
    }
}
