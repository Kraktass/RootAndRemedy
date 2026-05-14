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
        if (interactables.Count > 0) {
            SelectNearest();
        }
    }

    void OnTriggerEnter(Collider other) {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;
        if (!interactables.Contains(interactable))
            interactables.Add(interactable);
    }

    void OnTriggerExit(Collider other) {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;
        interactables.Remove(interactable);
        if (ReferenceEquals(interactable, currentInteractable))
            currentInteractable = null;
        if (ReferenceEquals(interactable, lastHighlighted))
            lastHighlighted.UnHighlight();
        lastHighlighted = null;
        SelectNearest();
    }

    public void SelectNearest() {
        IInteractable nearestTarget = null;
        Vector3 currentPosition = transform.position;
        float closestDistance = Mathf.Infinity;
        interactables.RemoveAll(i => i is not MonoBehaviour mb || mb == null);
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
        SelectNearest();
    }
}

