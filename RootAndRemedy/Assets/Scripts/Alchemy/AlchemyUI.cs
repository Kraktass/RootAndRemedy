using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyUI : MonoBehaviour {

    [SerializeField] private Transform ingredientListPanel;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;

    List<IngredientEntry> ingredientEntries = new List<IngredientEntry>();

    int selectedIngredientIndex = 0;
    int entryCount => ingredientEntries.Count;

    public bool isOpen => gameObject.activeSelf;

    void Awake() {
        ingredientEntries.Clear();
        ingredientEntries.AddRange(ingredientListPanel.GetComponentsInChildren<IngredientEntry>(true));
        Debug.Log(ingredientEntries.Count);
        HighlightIngredientEntryList();
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
