using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;
    private IInteractable currentInteractable;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;
        Debug.Log($"Interactable set: {interactable}");
    }

    public void ClearInteractable(IInteractable interactable)
    {
        if (currentInteractable == interactable)
        {
            currentInteractable = null;
            Debug.Log("Interactable cleared");
        }
    }

    public void TryInteract()
    {
        Debug.Log($"TryInteract - currentInteractable: {currentInteractable}");

        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
        else
        {
            Debug.Log("No interactable object nearby");
        }
    }
}
