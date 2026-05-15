
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;



public class InventoryInputBridge : MonoBehaviour {
    [SerializeField] Inventory inventory;
    [SerializeField] AlchemyUI alchemyUI;

    public void OnToggleInventory(InputValue _) {
        inventory.ToggleInventory();
    }

    public void OnPoint(InputValue value) {

        Vector2 pos = value.Get<Vector2>();
        inventory.SetPointerPosition(pos);
    }

    public void OnClick(InputValue value) {
        if (!inventory) return;

        bool pressed = value.Get<float>() > 0.5f;
        if (pressed)
            inventory.OnPrimaryClick();
    }

    public void OnInteract(InputValue _) {
        if (inventory.IsOpen) return;
    }

    public void OnHotbarScroll(InputValue value) {
        Vector2 scrollDirection = value.Get<Vector2>();
        if (scrollDirection.y > 0.5f) {
            inventory.SelectPreviousHotbarSlot();
        }
        else if (scrollDirection.y < -0.5f) {
            inventory.SelectNextHotbarSlot();
        }
    }

    public void OnAlchemyUIScroll(InputValue value) {
        if (!alchemyUI.isOpen) return;
        Vector2 scrollDirection = value.Get<Vector2>();
        if (scrollDirection.y > 0.5f) {
            alchemyUI.SelectPreviousEntry();
        }
        else if (scrollDirection.y < -0.5f) {
            alchemyUI.SelectNextEntry();
        }
    }
}
