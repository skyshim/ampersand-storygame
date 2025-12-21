using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryController : MonoBehaviour
{
    public static StoryController Instance;

    public DialogueUI dialogueUI;
    public CharacterManager characterManager;
    public CameraManager cameraManager;
    public BackgroundManager backgroundManager;

    [Header("Scene 순서")]
    public StoryScene[] storyScenes;

    private bool isProcessing = false;
    private int currentSceneIndex = 0;
    private bool dialogueInputReceived = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // 메시지가 떠 있으면 입력 허용
        if (dialogueUI != null && dialogueUI.IsShowingMessage)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                OnDialogueInput();
            }
        }

        if (isProcessing) return;
    }

    void OnDialogueInput()
    {
        dialogueInputReceived = true;
    }

    public void StartScene(int index)
    {
        if (isProcessing) return;
        if (index < 0 || index >= storyScenes.Length) return;

        currentSceneIndex = index;
        StartCoroutine(ExecuteScene(storyScenes[index]));
    }

    IEnumerator ExecuteScene(StoryScene scene)
    {
        isProcessing = true;
        Debug.Log($"▶ Scene Start : {scene.sceneName} (Type: {scene.sceneType})");

        // SceneType 변경 알림 (씬 시작할 때)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSceneTypeChanged(scene.sceneType);
        }

        foreach (var e in scene.events)
        {
            List<Coroutine> runningCoroutines = new();

            foreach (var action in e.actions)
            {
                switch (action.type)
                {
                    case StoryAction.ActionType.Dialogue:
                        ShowMessage(action);
                        yield return WaitForDialogueInput();
                        break;

                    case StoryAction.ActionType.Move:
                        runningCoroutines.Add(
                            StartCoroutine(characterManager.MoveCharacter(
                                action.characterName,
                                action.targetPosition,
                                action.moveDuration))
                        );
                        break;

                    case StoryAction.ActionType.Animate:
                        characterManager.PlayAnimation(
                            action.characterName,
                            action.animationTrigger);
                        break;

                    case StoryAction.ActionType.CameraMove:
                        runningCoroutines.Add(
                            StartCoroutine(cameraManager.MoveCamera(
                                action.cameraTargetPosition,
                                action.cameraMoveDuration,
                                action.cameraTargetSize))
                        );
                        break;

                    case StoryAction.ActionType.BackgroundChange:
                        backgroundManager.ChangeBackground(action.newBackground);
                        break;

                    case StoryAction.ActionType.Wait:
                        yield return new WaitForSeconds(action.waitDuration);
                        break;

                    case StoryAction.ActionType.SetCharacter:
                        characterManager.SetCurrentCharacter(action.playingCharacterName);
                        break;
                    case StoryAction.ActionType.SetSpriteVisibility:
                        SetSpriteVisibility(action.targetObjectName, action.targetAlpha);
                        break;
                }
            }

            foreach (var c in runningCoroutines)
                yield return c;
        }

        // PlayerControl 씬이면 FinishScene 호출하지 않고 대기
        if (scene.sceneType == SceneType.PlayerControl)
        {
            isProcessing = false;

            // 대화 UI 닫기
            if (dialogueUI != null)
            {
                dialogueUI.DisableDialogue();
            }

            Debug.Log("PlayerControl Scene - Enabling joystick and waiting for trigger");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDialogueFinished();
            }

            // 여기서 종료! Trigger가 올 때까지 대기
            yield break;
        }

        // PlayerControl이 아니면 일반적으로 FinishScene 호출
        FinishScene(scene);
    }

    void ShowMessage(StoryAction action)
    {
        var charData = characterManager.GetCharacter(action.characterName);

        dialogueUI.ShowMessage(
            action.messageType,
            action.dialogueText,
            action.characterName,
            charData?.portrait,
            charData?.transform
        );
    }

    IEnumerator WaitForDialogueInput()
    {
        // 타이핑 끝날 때까지 대기
        while (dialogueUI.IsTyping)
            yield return null;

        // 플래그 초기화
        dialogueInputReceived = false;

        // 사용자 클릭 대기
        while (!dialogueInputReceived)
            yield return null;
    }

    void SetSpriteVisibility(string objectName, float alpha)
    {
        // 오브젝트 찾기
        GameObject targetObj = GameObject.Find(objectName);
        if (targetObj == null)
        {
            Debug.LogWarning($"Object '{objectName}' not found!");
        }

        // SpriteRenderer 찾기
        SpriteRenderer spriteRenderer = targetObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"SpriteRenderer not found on '{objectName}'!");
        }

        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
        Debug.Log($"Set {objectName} visibility to {alpha} instantly");
    }



    void FinishScene(StoryScene scene)
    {
        isProcessing = false;

        // 대화 UI 닫기
        if (dialogueUI != null)
        {
            dialogueUI.DisableDialogue();
        }

        Debug.Log($"■ Scene End → {scene.sceneName} (Type: {scene.sceneType})");

        // GameManager에 씬 완료 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSceneComplete(currentSceneIndex);
        }

        // 다음 씬 처리
        if (scene.nextCondition == null)
        {
            Debug.Log("No next condition - scene complete");
            return;
        }

        switch (scene.nextCondition.type)
        {
            case NextCondition.ConditionType.Auto:
                Debug.Log("Auto condition - moving to next scene");
                currentSceneIndex++;
                if (currentSceneIndex < storyScenes.Length)
                {
                    StartScene(currentSceneIndex);
                }
                else
                {
                    Debug.Log("All scenes completed!");
                }
                break;

            case NextCondition.ConditionType.Trigger:
                Debug.Log("Trigger condition - waiting for TriggerNextScene call");
                // TriggerNextScene에서만 진행
                break;
        }
    }

    public void TriggerNextScene(string parameter = "")
    {
        Debug.Log($"TriggerNextScene called with parameter: '{parameter}'");

        if (isProcessing)
        {
            Debug.LogWarning("Scene is still processing, ignoring trigger");
            return;
        }

        if (currentSceneIndex >= storyScenes.Length)
        {
            Debug.LogWarning("No more scenes to trigger");
            return;
        }

        var scene = storyScenes[currentSceneIndex];
        if (scene.nextCondition == null)
        {
            Debug.LogWarning($"Scene '{scene.sceneName}' has no next condition");
            return;
        }

        if (scene.nextCondition.type != NextCondition.ConditionType.Trigger)
        {
            Debug.LogWarning($"Scene '{scene.sceneName}' is not a Trigger type (current: {scene.nextCondition.type})");
            return;
        }

        // 파라미터 체크
        if (!string.IsNullOrEmpty(scene.nextCondition.parameter))
        {
            if (scene.nextCondition.parameter != parameter)
            {
                Debug.LogWarning($"Parameter mismatch! Expected: '{scene.nextCondition.parameter}', Got: '{parameter}'");
                return;
            }
        }

        Debug.Log($"Trigger successful! Moving from scene {currentSceneIndex} to {currentSceneIndex + 1}");

        currentSceneIndex++;
        if (currentSceneIndex < storyScenes.Length)
        {
            StartScene(currentSceneIndex);
        }
        else
        {
            Debug.Log("All scenes completed!");
        }
    }
}