using UnityEngine;

public class PhotoInteraction : MonoBehaviour, IInteractable
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered Photo zone - registering interactable");
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
            Debug.Log("Player exited Photo zone - clearing interactable");
            if (InteractionManager.Instance != null)
            {
                InteractionManager.Instance.ClearInteractable(this);
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Photo Interact() called!");
        StoryController.Instance.TriggerNextScene("setCamera");
    }
}