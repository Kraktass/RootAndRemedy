using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInputBridge : MonoBehaviour {
    [SerializeField] Inventory inventory;

    public void OnToggleInventory(InputValue _) {
        inventory.ToggleInventory();
    }
}
