using System.Collections.Generic;
using UnityEngine;

public class CheckInteractable : MonoBehaviour {
    IInteractable currentInteractable;
    IInteractable lastHighlighted;
    PlayerMovement movement;

    public List<IInteractable> interactables = new();

    void Awake() {
        movement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if (movement.IsMoving && interactables.Count > 0) {
            SelectNearest();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<IInteractable>(out var interactable)) {
            if (interactable == null) return;
            interactables.Add(interactable);
            currentInteractable = interactable;
            //  Debug.Log("Interactable detected: " + ((MonoBehaviour)interactable).gameObject.name);
        }
    }

    void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent<IInteractable>(out var interactable)) return;
        interactables.Remove(interactable);
        if (interactable == currentInteractable) {
            currentInteractable = null;
        }
        //Debug.Log("Interactable left range: " + ((MonoBehaviour)interactable).gameObject.name);
        SelectNearest();
    }

    void SelectNearest() {
        IInteractable nearestTarget = null;
        Vector3 currentPosition = transform.position;
        float closestDistance = Mathf.Infinity;
        interactables.RemoveAll(item => item == null);
        foreach (IInteractable interactable in interactables) {
            Vector3 objectTransform = ((MonoBehaviour)interactable).transform.position;
            Vector3 distanceToTarget = objectTransform - currentPosition;
            float distanceSquaredToTarget = distanceToTarget.sqrMagnitude;
            if (interactable.CanInteract() && distanceSquaredToTarget < closestDistance) {
                closestDistance = distanceSquaredToTarget;
                nearestTarget = interactable;
            }
        }
        if (nearestTarget == lastHighlighted) return;
        lastHighlighted?.UnHighlight();
        nearestTarget?.Highlight();
        lastHighlighted = nearestTarget;
        currentInteractable = nearestTarget;
    }


    public void OnInteract() {
        var actedOn = currentInteractable;
        actedOn?.Interact();
        Debug.Log("You planted in: " + currentInteractable);
        SelectNearest();
    }

}
