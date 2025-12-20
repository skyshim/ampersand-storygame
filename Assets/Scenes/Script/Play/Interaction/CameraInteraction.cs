using UnityEngine;

public class CameraInteraction : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Interacted with Camera");

        StoryController.Instance.TriggerNextScene("findCamera");
        // ¿©±â¼­ ÇÒ °Íµé:
        // StoryController.Instance.StartScene(...)
        // ÄÆ¾À ½ÃÀÛ
        // UI ¶ç¿ì±â
    }
}
