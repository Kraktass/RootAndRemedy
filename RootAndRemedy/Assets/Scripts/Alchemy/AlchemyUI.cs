using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyUI : MonoBehaviour {

    [SerializeField] private Transform ingredientListPanel;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] Inventory inventory;
    [SerializeField] IngredientEntry ingredientEntry;
    [SerializeField] Transform ingredientPanel;

    List<IngredientEntry> ingredientEntries = new List<IngredientEntry>();

    int selectedIngredientIndex = 0;
    int entryCount => ingredientEntries.Count;

    public bool isOpen => gameObject.activeSelf;

    void Awake() {
        ingredientEntries.Clear();
        ingredientEntries.AddRange(ingredientListPanel.GetComponentsInChildren<IngredientEntry>(true));
        HighlightIngredientEntryList();
    }


    void OnEnable() {
        List<ItemStack> ingredients = inventory.GetAllIngredientItems();
        foreach (ItemStack item in ingredients) {
            IngredientEntry entry = Instantiate(ingredientEntry, ingredientListPanel);
            entry.SetData(item);

        }
    }

    void HighlightIngredientEntryList() {
        for (int i = 0; i < ingredientEntries.Count; i++) {

            Image img = ingredientEntries[i].GetComponent<Image>();
            img.color = (i == selectedIngredientIndex) ? selectedColor : normalColor;
        }
    }

    public void SelectPreviousEntry() {
        selectedIngredientIndex = (selectedIngredientIndex - 1 + entryCount) % entryCount;
        HighlightIngredientEntryList();
    }

    public void SelectNextEntry() {
        selectedIngredientIndex = (selectedIngredientIndex + 1) % entryCount;
        HighlightIngredientEntryList();
    }


}
