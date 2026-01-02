using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour {

    public ItemSO woodItem;
    public ItemSO axeItem;

    public GameObject hotbarObject;
    public GameObject inventorySlotParent;
    public GameObject container;

    public Image dragIcon;


    public float pickupRange;
    public Material highlightMaterial;

    Material originalMaterial;
    Renderer lookedAtRenderer = null;
    Item lookedAtItem = null;

    List<Slot> inventorySlots = new List<Slot>();
    List<Slot> hotbarSlots = new List<Slot>();
    List<Slot> allSlots = new List<Slot>();

    Slot draggedSlot;
    bool isDragging = false;

    public bool IsOpen => container != null && container.activeSelf;


    void Awake() {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        inventorySlots.AddRange(hotbarObject.GetComponentsInChildren<Slot>());

        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotbarSlots);

        container.SetActive(false);
    }

    void Update() {
        DetectLookedAtItem();
        Pickup();

        StartDrag();
        UpdateDragItemPosition();
        EndDrag();

    }

    public void ToggleInventory() {
        container.SetActive(!container.activeSelf);
    }


    public void AddItem(ItemSO itemToAdd, int amount) {

        int remaining = amount;

        foreach (Slot slot in allSlots) {
            if (slot.HasItem() && slot.GetItem() == itemToAdd) {
                int currentAmount = slot.GetAmount();
                int maxStack = itemToAdd.maxStackSize;

                if (currentAmount < maxStack) {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining <= 0) {
                        return;
                    }
                }
            }
        }

        foreach (Slot slot in allSlots) {
            if (!slot.HasItem()) {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;

                if (remaining <= 0) {
                    return;
                }
            }
        }

        if (remaining > 0)
            Debug.Log($"Inventory is full, could not add {remaining} of {itemToAdd.itemName}");

    }

    void StartDrag() {

        if (Input.GetMouseButtonDown(0)) {
            Slot hovered = GetHoveredSlot();

            if (hovered != null && hovered.HasItem()) {
                draggedSlot = hovered;
                isDragging = true;

                dragIcon.sprite = hovered.GetItem().itemIcon;
                dragIcon.color = new Color(1, 1, 1, 0.5f);
                dragIcon.enabled = true;
            }
        }

    }

    void EndDrag() {
        if (Input.GetMouseButtonUp(0) && isDragging) {
            Slot hovered = GetHoveredSlot();

            if (hovered != null) {
                HandleDrop(draggedSlot, hovered);

                dragIcon.enabled = false;
                draggedSlot = null;
                isDragging = false;
            }
        }
    }

    Slot GetHoveredSlot() {
        foreach (Slot s in allSlots) {
            if (s.hovering) {
                return s;
            }
        }
        return null;
    }

    void HandleDrop(Slot from, Slot to) {
        if (from == to) return;

        //Stacking
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

        //Different Item
        if (to.HasItem()) {
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetAmount();

            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
            return;
        }

        //Empty Slot
        to.SetItem(from.GetItem(), from.GetAmount());
        from.ClearSlot();
    }

    void UpdateDragItemPosition() {
        if (isDragging) {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    void Pickup() {
        if (lookedAtRenderer != null && Input.GetKeyDown(KeyCode.E)) {
            Item item = lookedAtRenderer.GetComponent<Item>();
            if (item != null) {
                AddItem(item.item, item.amount);
                Destroy(item.gameObject);
            }
        }
    }

    void DetectLookedAtItem() {
        if (lookedAtRenderer != null) {
            lookedAtRenderer.material = originalMaterial;
            lookedAtRenderer = null;
            originalMaterial = null;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange)) {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null) {
                Renderer rend = item.GetComponent<Renderer>();
                if (rend != null) {
                    originalMaterial = rend.material;
                    rend.material = highlightMaterial;
                    lookedAtRenderer = rend;
                }
            }
        }
    }
}
