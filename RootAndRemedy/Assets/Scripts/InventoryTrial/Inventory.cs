using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;


public class Inventory : MonoBehaviour {

    [SerializeField] private Color hotbarNormalColor = Color.white;
    [SerializeField] private Color hotbarSelectedColor = Color.yellow;

    public ItemSO woodItem;
    public ItemSO axeItem;

    public GameObject hotbarObject;
    public GameObject inventorySlotParent;
    public GameObject container;

    public Image dragIcon;


    public float pickupRange;
    [SerializeField] float raycastDistance = 100f;
    public Material highlightMaterial;

    Material originalMaterial;
    Renderer lookedAtRenderer = null;
    // Item lookedAtItem = null;

    List<Slot> inventorySlots = new List<Slot>();
    List<Slot> allSlots = new List<Slot>();
    List<Slot> hotbarSlots = new List<Slot>();

    Slot draggedSlot;
    bool isDragging = false;

    public bool IsOpen => container != null && container.activeSelf;

    [SerializeField] Transform player;
    [SerializeField] private LayerMask pickupMask = ~0;
    Vector2 pointerScreenPos;

    public int selectedHotbarIndex = 0;
    public int HotbarAmount => hotbarSlots.Count;


    void Awake() {
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
        DetectLookedAtItem();
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
        Debug.Log(selectedHotbarIndex);
    }

    public void SelectPreviousHotbarSlot() {
        selectedHotbarIndex = (selectedHotbarIndex - 1 + HotbarAmount) % HotbarAmount;
        HighlightHotbarSlot();
        Debug.Log(selectedHotbarIndex);
    }

    public void HighlightHotbarSlot() {
        for (int i = 0; i < hotbarSlots.Count; i++) {
            Image img = hotbarSlots[i].GetComponent<Image>();

            img.color = (i == selectedHotbarIndex) ? hotbarSelectedColor : hotbarNormalColor;
        }
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

    public void OnClickPressed() {
        if (!IsOpen) return;

        Slot hovered = GetHoveredSlot();
        Debug.Log("Hovered on release: " + (hovered ? hovered.name : "NULL"));
        if (hovered != null && hovered.HasItem()) {
            draggedSlot = hovered;
            isDragging = true;

            dragIcon.sprite = hovered.GetItem().itemIcon;
            dragIcon.color = new Color(1, 1, 1, 0.5f);
            dragIcon.enabled = true;

            UpdateDragItemPosition();
        }
    }

    public void OnClickReleased() {
        if (!IsOpen) return;
        if (!isDragging) return;

        Slot hovered = GetHoveredSlot();
        Debug.Log("Hovered on release: " + (hovered ? hovered.name : "NULL"));
        if (hovered != null) {
            HandleDrop(draggedSlot, hovered);
        }

        dragIcon.enabled = false;
        draggedSlot = null;
        isDragging = false;
    }

    public void OnPrimaryClick() {
        if (!IsOpen) return;

        Slot hovered = GetHoveredSlot();
        if (hovered == null) return;

        // If not currently holding/dragging: pick up from clicked slot
        if (!isDragging) {
            if (!hovered.HasItem()) return;

            draggedSlot = hovered;
            isDragging = true;

            dragIcon.sprite = hovered.GetItem().itemIcon;
            dragIcon.color = new Color(1, 1, 1, 0.5f);
            dragIcon.enabled = true;

            // Optional: snap icon immediately
            dragIcon.transform.position = pointerScreenPos;
            return;
        }

        // If currently holding/dragging: place into clicked slot
        HandleDrop(draggedSlot, hovered);

        dragIcon.enabled = false;
        draggedSlot = null;
        isDragging = false;
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
        if (isDragging)
            dragIcon.transform.position = pointerScreenPos;
    }

    public void SetPointerPosition(Vector2 screenPos) {
        pointerScreenPos = screenPos;
    }


    public void TryPickup() {
        if (lookedAtRenderer == null) return;

        Item item = lookedAtRenderer.GetComponent<Item>();
        if (item == null) return;

        if (player == null) {
            Debug.LogError("Inventory.TryPickup: Player Transform is not assigned.");
            return;
        }

        float dist = Vector3.Distance(player.position, item.transform.position);
        Debug.Log($"Pickup check: player={player?.name}, item={item.name}, dist={dist}, range={pickupRange}");
        if (dist > pickupRange)
            return;

        AddItem(item.item, item.amount);
        Destroy(item.gameObject);
    }

    void DetectLookedAtItem() {
        if (lookedAtRenderer != null) {
            lookedAtRenderer.material = originalMaterial;
            lookedAtRenderer = null;
            originalMaterial = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(pointerScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, pickupMask)) {
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
