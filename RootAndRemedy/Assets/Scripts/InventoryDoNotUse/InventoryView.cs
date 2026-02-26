using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour {
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private InventorySlotView slotPrefab;
    [SerializeField] private Transform slotsParent;

    private readonly List<InventorySlotView> views = new();

    void Awake() {
        if (!inventory)
            inventory = FindFirstObjectByType<PlayerInventory>();

    }

    void Start() {
        Build();
        if (inventory) inventory.RaiseFullRefresh();
    }

    void OnEnable() {
        if (!inventory) return;

        inventory.OnSlotChanged += HandleSlotChanged;
        inventory.OnSelectedHotbarChanged += HandleSelectedHotbarChanged;

        inventory.RaiseFullRefresh();
    }

    void OnDisable() {
        if (!inventory) return;

        inventory.OnSlotChanged -= HandleSlotChanged;
        inventory.OnSelectedHotbarChanged -= HandleSelectedHotbarChanged;
    }

    private void Build() {
        if (!inventory || !slotPrefab || !slotsParent) return;
        Debug.Log($"InventoryView.Build: inventory={inventory != null}, prefab={slotPrefab != null}, parent={slotsParent != null}");
        Debug.Log($"InventoryView.Build: creating {inventory.InventorySize} slots under {slotsParent.name}");


        // Clear existing children
        for (int i = slotsParent.childCount - 1; i >= 0; i--)
            Destroy(slotsParent.GetChild(i).gameObject);

        views.Clear();

        for (int i = 0; i < inventory.InventorySize; i++) {
            InventorySlotView view = Instantiate(slotPrefab, slotsParent);
            view.SetSelected(false);
            view.Render(inventory.GetSlot(i));
            views.Add(view);
        }
    }

    private void HandleSlotChanged(int index, InventorySlot slot) {
        if (index < 0 || index >= views.Count) return;
        views[index].Render(slot);
    }

    private void HandleSelectedHotbarChanged(int selectedHotbarIndex) {
        int hotbarSize = inventory.HotbarSize;

        for (int i = 0; i < views.Count; i++) {
            bool isHotbarSlot = i < hotbarSize;
            bool selected = isHotbarSlot && (i == selectedHotbarIndex);
            views[i].SetSelected(selected);
        }
    }
}
