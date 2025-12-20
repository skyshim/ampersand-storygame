using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    private IInteractable currentInteractable;

    private void Awake()
    {
        Instance = this;
    }

    public void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;
        Debug.Log("Interactable");
    }

    public void ClearInteractable(IInteractable interactable)
    {
        if (currentInteractable == interactable)
            currentInteractable = null;
    }

    public void TryInteract()
    {
        if (currentInteractable == null) return;
        currentInteractable.Interact();
    }
}
