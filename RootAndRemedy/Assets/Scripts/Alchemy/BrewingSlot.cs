
using UnityEngine;
using UnityEngine.UI;

public class BrewingSlot : MonoBehaviour {

    [SerializeField] private Image icon;

    public ItemSO item;

    public void SetItem(ItemSO item) {
        Debug.Log("Brewing slot received: " + item.itemName);
        this.item = item;
        icon.sprite = item.itemIcon;
    }

}
