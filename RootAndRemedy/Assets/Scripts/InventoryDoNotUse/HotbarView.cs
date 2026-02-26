using System.Collections.Generic;
using UnityEngine;

public class HotbarView : MonoBehaviour {
    [Header("Wiring")]
    [SerializeField] private PlayerInventory inventory;

    [Header("UI")]
    [SerializeField] private InventorySlotView slotPrefab;
    [SerializeField] private Transform hotbarParent;

    private readonly List<InventorySlotView> views = new();

    void Awake() {
        if (!inventory)
            inventory = FindFirstObjectByType<PlayerInventory>();

        // If you forget to assign hotbarParent, we can safely default to "this"
        if (!hotbarParent)
            hotbarParent = transform;
    }

    void Start() {
        BuildHotbar();
        if (inventory) inventory.RaiseFullRefresh();
    }

    void OnEnable() {
        if (!inventory) return;

        inventory.OnSlotChanged += HandleSlotChanged;
        inventory.OnSelectedHotbarChanged += HandleSelectedHotbarChanged;
    }

    void OnDisable() {
        if (!inventory) return;

        inventory.OnSlotChanged -= HandleSlotChanged;
        inventory.OnSelectedHotbarChanged -= HandleSelectedHotbarChanged;
    }

    private void BuildHotbar() {
        if (!inventory || !slotPrefab || !hotbarParent) return;

        // Clear existing children
        for (int i = hotbarParent.childCount - 1; i >= 0; i--)
            Destroy(hotbarParent.GetChild(i).gameObject);

        views.Clear();

        int count = inventory.HotbarSize;

        for (int i = 0; i < count; i++) {
            InventorySlotView view = Instantiate(slotPrefab, hotbarParent);
            view.SetSelected(false);
            view.Render(inventory.GetSlot(i));   // hotbar = first N slots
            views.Add(view);
        }

        // Ensure selection highlight is correct immediately
        HandleSelectedHotbarChanged(inventory.SelectedHotbarIndex);
    }

    private void HandleSlotChanged(int index, InventorySlot slot) {
        // Hotbar only cares about first N slots
        if (index < 0 || index >= views.Count) return;
        views[index].Render(slot);
    }

    private void HandleSelectedHotbarChanged(int selectedHotbarIndex) {
        for (int i = 0; i < views.Count; i++) {
            views[i].SetSelected(i == selectedHotbarIndex);
        }
    }
}

