using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private int size = 36;

    [Header("Debug View (runtime)")]
    [SerializeField] private InventorySlot[] slots;

    [Header("Debug Add (Inspector)")]
    [SerializeField] private ItemData debugItem;
    [SerializeField] private int debugAmount = 20;

    [Header("Hotbar")]
    [SerializeField] private int hotbarSize = 9;
    [SerializeField] private int selectedHotbarIndex = 0;


    void Awake() {
        slots = new InventorySlot[size];
        for (int i = 0; i < size; i++)
            slots[i] = new InventorySlot();
    }

    public int HotbarSize => Mathf.Min(hotbarSize, slots.Length);
    public int SelectedHotbarIndex => Mathf.Clamp(selectedHotbarIndex, 0, HotbarSize - 1);

    public InventorySlot GetSelectedHotbarSlot() {
        return slots[SelectedHotbarIndex];
    }

    public void SelectNextHotbarSlot() {
        int size = HotbarSize;
        if (size <= 0) return;

        selectedHotbarIndex = (selectedHotbarIndex + 1) % size;
        DebugPrintSelected();
    }

    public void SelectPreviousHotbarSlot() {
        int size = HotbarSize;
        if (size <= 0) return;

        selectedHotbarIndex = (selectedHotbarIndex - 1 + size) % size;
        DebugPrintSelected();
    }

    private void DebugPrintSelected() {
        Debug.Log($"Selected hotbar index: {SelectedHotbarIndex}");
    }

    public bool Add(ItemData itemToAdd, int quantityToAdd) {

        if (itemToAdd == null || quantityToAdd <= 0) return false;

        int remainingQuantity = quantityToAdd;

        for (int i = 0; i < slots.Length; i++) {
            InventorySlot currentSlot = slots[i];
            if (currentSlot.IsEmpty) continue;
            if (currentSlot.item != itemToAdd) continue;
            if (currentSlot.count >= itemToAdd.maxStackSize) continue;

            int availableSpace = itemToAdd.maxStackSize - currentSlot.count;
            int quantityToMove = Mathf.Min(availableSpace, remainingQuantity);

            currentSlot.count += quantityToMove;
            remainingQuantity -= quantityToMove;

            if (remainingQuantity <= 0) return true;
        }

        for (int i = 0; i < slots.Length; i++) {
            InventorySlot currentSlot = slots[i];

            if (!currentSlot.IsEmpty) continue;

            currentSlot.item = itemToAdd;

            int quantityToMove = Mathf.Min(itemToAdd.maxStackSize, remainingQuantity);
            currentSlot.count = quantityToMove;
            remainingQuantity -= quantityToMove;

            if (remainingQuantity <= 0) return true;
        }
        return false;
    }

    [ContextMenu("Debug: Add Debug Item")]
    private void DebugAdd() {
        if (debugItem == null) {
            Debug.LogWarning("Debug Item is not assigned.");
            return;
        }

        bool addedAll = Add(debugItem, debugAmount);

        Debug.Log(addedAll
            ? $"Added {debugAmount} of {debugItem.displayName}"
            : $"Inventory full: could not add all {debugAmount} of {debugItem.displayName}");
    }

    [ContextMenu("Debug: Print Inventory")]
    private void DebugPrint() {
        for (int slotIndex = 0; slotIndex < slots.Length; slotIndex++) {
            InventorySlot slot = slots[slotIndex];
            if (slot.IsEmpty) continue;

            Debug.Log($"Slot {slotIndex}: {slot.item.displayName} x{slot.count}");
        }
    }
}
