using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarSelectionInput : MonoBehaviour {

    [SerializeField] private PlayerInventory inventory;

    void Awake() {
        if (!inventory)
            inventory = GetComponent<PlayerInventory>();
    }

    public void OnHotBarNext(InputValue _) {
        if (inventory == null) return;
        inventory.SelectNextHotbarSlot();
    }

    public void OnHotbarPrevious(InputValue _) {
        if (inventory == null) return;
        inventory.SelectPreviousHotbarSlot();
    }

    public void OnHotbarScroll(InputValue value) {
        if (inventory == null) return;

        Vector2 scroll = value.Get<Vector2>();
        if (scroll.y > 0f) {
            inventory.SelectPreviousHotbarSlot();
        }
        if (scroll.y < 0f) {
            inventory.SelectNextHotbarSlot();
        }
    }
}
