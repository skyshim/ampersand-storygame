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

    // 입력 플래그 (추가)
    private bool dialogueInputReceived = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // SceneType 무시하고, 메시지가 떠 있으면 입력 허용
        if (dialogueUI != null && dialogueUI.IsShowingMessage)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                OnDialogueInput();
            }
        }

        if (isProcessing) return;
    }

    // 빠졌던 함수 완성
    void OnDialogueInput()
    {
        dialogueInputReceived = true;
    }

    public void StartScene(int index)
    {
        if (isProcessing) return;
        if (index < 0 || index >= storyScenes.Length) return;

        StartCoroutine(ExecuteScene(storyScenes[index]));
    }

    IEnumerator ExecuteScene(StoryScene scene)
    {
        isProcessing = true;
        Debug.Log($"▶ Scene Start : {scene.sceneName}");

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
                }
            }

            foreach (var c in runningCoroutines)
                yield return c;
        }

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
        while (dialogueUI.IsTyping)
            yield return null;

        // 플래그 초기화
        dialogueInputReceived = false;

        while (!dialogueInputReceived)
            yield return null;
    }

    void FinishScene(StoryScene scene)
    {
        isProcessing = false;

        if (scene.nextCondition == null)
            return;

        switch (scene.nextCondition.type)
        {
            case NextCondition.ConditionType.Auto:
                currentSceneIndex++;
                StartScene(currentSceneIndex);
                break;

            case NextCondition.ConditionType.Trigger:
                // TriggerNextScene에서만 진행
                break;
        }

        GameManager.Instance.OnSceneComplete(currentSceneIndex);
        Debug.Log($"■ Scene End → {currentSceneIndex}");
    }

    public void TriggerNextScene(string parameter = "")
    {
        if (isProcessing) return;

        var scene = storyScenes[currentSceneIndex];
        if (scene.nextCondition == null) return;
        if (scene.nextCondition.type != NextCondition.ConditionType.Trigger) return;

        if (!string.IsNullOrEmpty(scene.nextCondition.parameter) &&
            scene.nextCondition.parameter != parameter) return;

        isProcessing = true;
        currentSceneIndex++;
        StartCoroutine(ExecuteScene(storyScenes[currentSceneIndex]));
    }
}
