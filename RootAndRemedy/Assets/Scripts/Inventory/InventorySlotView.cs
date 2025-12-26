using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour {

    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private GameObject selectionHighlight;

    public void SetSelected(bool selected) {

        if (selectionHighlight)
            selectionHighlight.SetActive(selected);

    }

    public void Render(InventorySlot slot) {
        bool empty = (slot == null) || slot.IsEmpty;

        if (iconImage) {
            iconImage.enabled = !empty;
            iconImage.sprite = empty ? null : slot.item.icon;
        }

        if (countText) {
            countText.text = (empty || slot.count <= 1) ? "" : slot.count.ToString();
        }
    }
}
