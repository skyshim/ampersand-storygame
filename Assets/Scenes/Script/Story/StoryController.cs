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

    [Header("Scene 순서대로 불러오기 (Resources/StoryScenes 폴더)")]
    public StoryScene[] storyScenes;

    public bool isProcessing = false;
    private int currentSceneIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isProcessing) return;

        if (currentSceneIndex < storyScenes.Length &&
            storyScenes[currentSceneIndex].sceneType == SceneType.Dialogue)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                OnClick();
            }
        }
    }

    private void OnClick()
    {
        if (isProcessing) return;
        if (currentSceneIndex >= storyScenes.Length) return;

        isProcessing = true;
        StartCoroutine(ExecuteScene(storyScenes[currentSceneIndex]));
    }

    public void StartScene(int index)
    {
        if (index < 0 || index >= storyScenes.Length) return;
        currentSceneIndex = index;
        StartCoroutine(ExecuteScene(storyScenes[currentSceneIndex]));
    }

    private IEnumerator ExecuteScene(StoryScene scene)
    {
        Debug.Log(scene.sceneName);
        isProcessing = true;

        dialogueUI.panel.SetActive(scene.sceneType == SceneType.Dialogue);
        dialogueUI.balloonRoot.SetActive(scene.sceneType == SceneType.Dialogue);

        foreach (var e in scene.events)
        {
            List<Coroutine> runningCoroutines = new List<Coroutine>();

            foreach (var action in e.actions)
            {
                switch (action.type)
                {
                    case StoryAction.ActionType.Dialogue:
                        var charData = characterManager.GetCharacter(action.characterName);
                        dialogueUI.ShowDialogue(action.characterName, action.dialogueText, action.isBalloon, charData?.portrait, charData?.transform);

                        while (dialogueUI.IsTyping)
                            yield return null;

                        bool clicked = false;
                        while (!clicked)
                        {
                            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                                clicked = true;
                            yield return null;
                        }
                        break;

                    case StoryAction.ActionType.Move:
                        runningCoroutines.Add(StartCoroutine(characterManager.MoveCharacter(action.characterName, action.targetPosition, action.moveDuration)));
                        break;

                    case StoryAction.ActionType.Animate:
                        characterManager.PlayAnimation(action.characterName, action.animationTrigger);
                        break;

                    case StoryAction.ActionType.CameraMove:
                        runningCoroutines.Add(
                            StartCoroutine(CameraManager.Instance.MoveCamera(
                                action.cameraTargetPosition,
                                action.cameraMoveDuration,
                                action.cameraTargetSize
                            ))
                        );
                        break;

                    case StoryAction.ActionType.BackgroundChange:
                        BackgroundManager.Instance.ChangeBackground(action.newBackground);
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

        int nextIndex = currentSceneIndex;
        if (scene.nextCondition != null && scene.nextCondition.type == NextCondition.ConditionType.Auto)
            nextIndex++;

        isProcessing = false;
        GameManager.Instance.OnSceneComplete(nextIndex);
        Debug.Log("ExecuteScene finished, nextIndex = " + nextIndex);

        if (nextIndex < storyScenes.Length)
        {
            var nextScene = storyScenes[nextIndex];
            GameManager.Instance.OnSceneTypeChanged(nextScene.sceneType);

            if (nextScene.sceneType == SceneType.PlayerControl)
            {
                // PlayerControl 씬인데 대화까지 끝났다면
                dialogueUI.DisableDialogue();
                GameManager.Instance.OnDialogueFinished();
            }
        }
    }

    public void TriggerNextScene(string parameter = "")
    {
        if (isProcessing) return;

        var scene = storyScenes[currentSceneIndex];
        if (scene.nextCondition == null) return;
        if (scene.nextCondition.type != NextCondition.ConditionType.Trigger)
            return;

        if (!string.IsNullOrEmpty(scene.nextCondition.parameter) &&
            scene.nextCondition.parameter != parameter)
            return;

        isProcessing = true;
        currentSceneIndex++;

        StartCoroutine(ExecuteScene(storyScenes[currentSceneIndex]));
    }
}