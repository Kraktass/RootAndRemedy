using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour {

    [SerializeField] private Color hotbarNormalColor = Color.white;
    [SerializeField] private Color hotbarSelectedColor = Color.yellow;

    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    public ItemSO woodItem;
    public ItemSO axeItem;

    public GameObject hotbarObject;
    public GameObject inventorySlotParent;
    public GameObject container;

    public Image dragIcon;

    List<Slot> inventorySlots = new List<Slot>();
    List<Slot> allSlots = new List<Slot>();
    List<Slot> hotbarSlots = new List<Slot>();

    Slot draggedSlot;
    bool isDragging = false;

    public bool IsOpen => container != null && container.activeSelf;

    Vector2 pointerScreenPos;

    public int selectedHotbarIndex = 0;
    public int HotbarAmount => hotbarSlots.Count;

    void Awake() {
        if (!eventSystem)
            eventSystem = EventSystem.current;

        if (!raycaster)
            raycaster = GetComponentInParent<Canvas>()?.GetComponent<GraphicRaycaster>();

        if (!raycaster)
            Debug.LogError("Inventory: No GraphicRaycaster found. Is this Inventory under a Canvas?");

        inventorySlots.Clear();
        hotbarSlots.Clear();
        allSlots.Clear();

        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>(true));
        hotbarSlots.AddRange(hotbarObject.GetComponentsInChildren<Slot>(true));

        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotbarSlots);

        HighlightHotbarSlot();

        container.SetActive(false);
    }

    void Update() {
        if (isDragging) {
            UpdateDragItemPosition();
        }
    }

    public void ToggleInventory() {
        container.SetActive(!container.activeSelf);
    }

    public void SelectNextHotbarSlot() {
        selectedHotbarIndex = (selectedHotbarIndex + 1) % HotbarAmount;
        HighlightHotbarSlot();
    }

    public void SelectPreviousHotbarSlot() {
        selectedHotbarIndex = (selectedHotbarIndex - 1 + HotbarAmount) % HotbarAmount;
        HighlightHotbarSlot();
    }

    public void HighlightHotbarSlot() {
        for (int i = 0; i < hotbarSlots.Count; i++) {
            Image img = hotbarSlots[i].GetComponent<Image>();
            img.color = (i == selectedHotbarIndex) ? hotbarSelectedColor : hotbarNormalColor;
        }
    }

    public ItemStack GetSelectedItemStack() {
        Slot slot = hotbarSlots[selectedHotbarIndex];
        if (!slot.HasItem()) {
            return null;
        }

        return new ItemStack {
            item = slot.GetItem(),
            amount = slot.GetAmount()
        };
    }

    public void ConsumeSelectedItem() {
        Slot slot = hotbarSlots[selectedHotbarIndex];
        if (slot.HasItem()) {
            slot.RemoveAmount(1);
        }
    }

    public void AddItem(ItemSO itemToAdd, int amount) {
        int remaining = amount;

        foreach (Slot slot in hotbarSlots) {
            if (!slot.HasItem()) continue;
            if (slot.GetItem() != itemToAdd) continue;

            int currentAmount = slot.GetAmount();
            int maxStack = itemToAdd.maxStackSize;
            if (currentAmount >= maxStack) continue;

            int spaceLeft = maxStack - currentAmount;
            int amountToAdd = Mathf.Min(spaceLeft, remaining);

            slot.SetItem(itemToAdd, currentAmount + amountToAdd);
            remaining -= amountToAdd;

            if (remaining <= 0)
                return;
        }

        foreach (Slot slot in hotbarSlots) {
            if (slot.HasItem()) continue;

            int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
            slot.SetItem(itemToAdd, amountToPlace);
            remaining -= amountToPlace;

            if (remaining <= 0)
                return;
        }

        foreach (Slot slot in inventorySlots) {
            if (!slot.HasItem()) continue;
            if (slot.GetItem() != itemToAdd) continue;

            int currentAmount = slot.GetAmount();
            int maxStack = itemToAdd.maxStackSize;
            if (currentAmount >= maxStack) continue;

            int spaceLeft = maxStack - currentAmount;
            int amountToAdd = Mathf.Min(spaceLeft, remaining);

            slot.SetItem(itemToAdd, currentAmount + amountToAdd);
            remaining -= amountToAdd;

            if (remaining <= 0)
                return;
        }

        foreach (Slot slot in inventorySlots) {
            if (slot.HasItem()) continue;

            int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
            slot.SetItem(itemToAdd, amountToPlace);
            remaining -= amountToPlace;

            if (remaining <= 0)
                return;
        }

        if (remaining > 0)
            Debug.Log($"Inventory is full, could not add {remaining} of {itemToAdd.itemName}");
    }

    private Slot GetSlotUnderPointer() {
        if (!raycaster || !eventSystem) return null;

        var ped = new PointerEventData(eventSystem) { position = pointerScreenPos };
        var results = new List<RaycastResult>();
        raycaster.Raycast(ped, results);

        foreach (var r in results) {
            Slot slot = r.gameObject.GetComponentInParent<Slot>();
            if (slot != null)
                return slot;
        }

        return null;
    }

    public void OnPrimaryClick() {
        Slot hovered = GetSlotUnderPointer();
        if (!IsOpen) return;
        if (hovered == null) return;

        if (!isDragging) {
            if (!hovered.HasItem()) return;

            draggedSlot = hovered;
            isDragging = true;

            dragIcon.sprite = hovered.GetItem().itemIcon;
            dragIcon.color = new Color(1, 1, 1, 0.5f);
            dragIcon.enabled = true;
            dragIcon.transform.position = pointerScreenPos;
            return;
        }

        HandleDrop(draggedSlot, hovered);

        dragIcon.enabled = false;
        draggedSlot = null;
        isDragging = false;
    }

    void HandleDrop(Slot from, Slot to) {
        if (from == to) return;

        if (to.HasItem() && to.GetItem() == from.GetItem()) {
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetAmount();

            if (space > 0) {
                int move = Mathf.Min(space, from.GetAmount());

                to.SetItem(to.GetItem(), to.GetAmount() + move);
                from.SetItem(from.GetItem(), from.GetAmount() - move);

                if (from.GetAmount() <= 0) {
                    from.ClearSlot();
                }
                return;
            }
        }

        if (to.HasItem()) {
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetAmount();

            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
            return;
        }

        to.SetItem(from.GetItem(), from.GetAmount());
        from.ClearSlot();
    }

    void UpdateDragItemPosition() {
        if (isDragging)
            dragIcon.transform.position = pointerScreenPos;
    }

    public void SetPointerPosition(Vector2 screenPos) {
        pointerScreenPos = screenPos;
    }
}