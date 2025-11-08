using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryController : MonoBehaviour
{
    public static StoryController Instance;

    public DialogueUI dialogueUI;
    public CharacterManager characterManager;

    [Header("Scene 순서대로 불러오기 (Resources/StoryScenes 폴더)")]
    public StoryScene[] storyScenes;

    private int currentSceneIndex = 0;
    private bool isProcessing = false;

    private void Awake()
    {
        Instance = this;

        // Resources 폴더에서 Scene Asset 로드
        storyScenes = Resources.LoadAll<StoryScene>("StoryScenes");
    }

    private void Update()
    {
        if (isProcessing) return;
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            OnClick();
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
        isProcessing = true;

        foreach (var e in scene.events)
        {
            List<Coroutine> runningCoroutines = new List<Coroutine>();

            foreach (var action in e.actions)
            {
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
