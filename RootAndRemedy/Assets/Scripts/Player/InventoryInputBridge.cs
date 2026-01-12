
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;


public class InventoryInputBridge : MonoBehaviour {
    [SerializeField] Inventory inventory;

    public void OnToggleInventory(InputValue _) {
        inventory.ToggleInventory();
    }

    public void OnPoint(InputValue value) {

        Vector2 pos = value.Get<Vector2>();
        inventory.SetPointerPosition(pos);
    }

    public void OnInteract() {
        if (inventory.IsOpen) return;
        inventory.TryPickup();
    }

    public void OnClick(InputValue value) {
        if (!inventory) return;

        bool pressed = value.Get<float>() > 0.5f;
        if (pressed)
            inventory.OnPrimaryClick();
    }

    public void OnInteract(InputValue _) {
        if (inventory.IsOpen) return;
        inventory.TryPickup();
    }

    public void OnHotbarScroll(InputValue value) {
        Vector2 scrollDirection = value.Get<Vector2>();
        if (scrollDirection.y > 0.5f) {
            inventory.SelectNextHotbarSlot();
        }
        else if (scrollDirection.y < -0.5f) {
            inventory.SelectPreviousHotbarSlot();
        }
    }
}
