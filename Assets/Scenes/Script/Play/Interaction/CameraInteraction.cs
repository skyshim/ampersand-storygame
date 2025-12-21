using UnityEngine;

public class CameraInteraction : MonoBehaviour, IInteractable
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered Camera zone - registering interactable");
            if (InteractionManager.Instance != null)
            {
                InteractionManager.Instance.SetInteractable(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited Camera zone - clearing interactable");
            if (InteractionManager.Instance != null)
            {
                InteractionManager.Instance.ClearInteractable(this);
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Camera Interact() called!");
        StoryController.Instance.TriggerNextScene("findCamera");
    }
}