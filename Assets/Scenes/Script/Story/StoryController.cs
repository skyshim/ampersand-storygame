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

        bool isDialogueScene = scene.sceneType == SceneType.Dialogue;
        if (isDialogueScene)
            dialogueUI.panel.SetActive(true);
        else
            dialogueUI.panel.SetActive(false);

        foreach (var e in scene.events)
        {
            List<Coroutine> runningCoroutines = new List<Coroutine>();

            foreach (var action in e.actions)
            {
                Debug.Log(action.type);
                switch (action.type)
                {
                    case StoryAction.ActionType.Dialogue:
                        var charData = characterManager.GetCharacter(action.characterName);
                        dialogueUI.ShowDialogue(action.characterName, action.dialogueText, charData?.portrait);

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
                        Debug.Log("CameraMove: " + action.cameraTargetPosition);
                        break;

                    case StoryAction.ActionType.BackgroundChange:
                        BackgroundManager.Instance.ChangeBackground(action.newBackground);
                        Debug.Log("BackgroundChange: " + action.newBackground.name);
                        break;

                    case StoryAction.ActionType.Wait:
                        yield return new WaitForSeconds(action.waitDuration);
                        break;
                }
            }

            // 병렬 Move 액션 끝날 때까지 대기
            foreach (var c in runningCoroutines)
                yield return c;
        }

        // Scene 종료 후 GameManager에게 알림
        int nextIndex = currentSceneIndex;
        if (scene.nextCondition != null && scene.nextCondition.type == NextCondition.ConditionType.Auto)
            nextIndex++;

        isProcessing = false;
        GameManager.Instance.OnSceneComplete(nextIndex);
    }
}
