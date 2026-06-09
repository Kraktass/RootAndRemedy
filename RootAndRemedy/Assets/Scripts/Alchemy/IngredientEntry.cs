
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientEntry : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text amount;
    [SerializeField] private ItemSO item;

    public void SetData(ItemStack stack) {
        icon.sprite = stack.item.itemIcon;
        label.text = stack.item.itemName;
        amount.text = stack.amount.ToString();
        item = stack.item;
    }

    public ItemSO GetItem() {
        return item;
    }
}
