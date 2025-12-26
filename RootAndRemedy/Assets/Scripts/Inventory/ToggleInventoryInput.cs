using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggleInput : MonoBehaviour {
    [SerializeField] private GameObject inventoryUIRoot;

    private bool isOpen;

    void Awake() {
        if (!inventoryUIRoot)
            Debug.LogWarning("InventoryToggleInput: inventoryUIRoot not assigned.");
    }

    void Start() {
        // Optional default: start closed
        if (inventoryUIRoot)
            inventoryUIRoot.SetActive(false);

        isOpen = false;
    }

    // PlayerInput (Send Messages) will call this when ToggleInventory fires
    public void OnToggleInventory(InputValue _) {
        Toggle();
    }

    private void Toggle() {
        if (!inventoryUIRoot) return;

        isOpen = !isOpen;
        inventoryUIRoot.SetActive(isOpen);
    }
}

