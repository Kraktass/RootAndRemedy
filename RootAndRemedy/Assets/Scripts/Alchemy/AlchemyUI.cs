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
    [SerializeField] BrewingSlot brewingSlotOne;
    [SerializeField] BrewingSlot brewingSlotTwo;
    [SerializeField] Slot resultSlot;
    [SerializeField] PotionRecipeDatabase potionRecipeDatabase;


    List<IngredientEntry> ingredientEntries = new List<IngredientEntry>();

    BrewingSlot[] brewingSlots;
    int activeBrewingSlotIndex = 0;
    int selectedIngredientIndex = 0;
    int entryCount => ingredientEntries.Count;
    public bool isOpen => gameObject.activeSelf;


    private List<ItemSO> GetSelectedIngredients() {
        List<ItemSO> selectedIngredients = new();

        foreach (BrewingSlot slot in brewingSlots) {
            if (slot.item == null)
                continue;

            if (selectedIngredients.Contains(slot.item))
                continue;

            selectedIngredients.Add(slot.item);
        }

        return selectedIngredients;
    }


    void Awake() {
        ingredientEntries.Clear();
        ingredientEntries.AddRange(ingredientListPanel.GetComponentsInChildren<IngredientEntry>(true));
        HighlightIngredientEntryList();
        brewingSlots = new[] { brewingSlotOne, brewingSlotTwo };
    }

    private void ClearIngredientEntries() {
        foreach (IngredientEntry entry in ingredientEntries) {
            if (entry != null)
                Destroy(entry.gameObject);
        }

        ingredientEntries.Clear();
    }

    void OnEnable() {
        RefreshIngredientList();
    }

    void RefreshIngredientList() {
        ClearIngredientEntries();
        List<ItemStack> ingredients = inventory.GetAllIngredientItems();
        foreach (ItemStack item in ingredients) {
            IngredientEntry entry = Instantiate(ingredientEntry, ingredientListPanel);
            entry.SetData(item);
            ingredientEntries.Add(entry);
        }
        if (ingredients.Count > 0) {
            HighlightIngredientEntryList();
        }
    }

    public void SetItem() {
        IngredientEntry selectedEntry = ingredientEntries[selectedIngredientIndex];
        ItemSO selectedItem = selectedEntry.GetItem();
        Debug.Log("Selected: " + selectedItem.itemName);
        brewingSlots[activeBrewingSlotIndex].SetItem(selectedItem);
        if (activeBrewingSlotIndex == 0) {
            activeBrewingSlotIndex++;
        }
        else {
            activeBrewingSlotIndex--;
        }
    }

    public void BrewPotion() {
        List<ItemSO> selectedIngredients = GetSelectedIngredients();

        PotionRecipeSO recipe =
            potionRecipeDatabase.GetRecipe(selectedIngredients);

        if (recipe == null) {
            Debug.Log("No matching recipe!");
            return;
        }

        if (resultSlot.HasItem() && resultSlot.GetItem() != recipe.result) {
            Debug.Log("Take the existing potion first!");
            return;
        }

        foreach (RecipeIngredient ingredient in recipe.ingredients) {
            if (!inventory.HasEnoughItems(ingredient.item, ingredient.amount)) {
                Debug.Log($"Not enough {ingredient.item.itemName}!");
                return;
            }
        }

        foreach (RecipeIngredient ingredient in recipe.ingredients) {
            inventory.ConsumeItem(ingredient.item, ingredient.amount);
        }

        if (!resultSlot.HasItem()) {
            resultSlot.SetItem(recipe.result, 1);
        }
        else {
            resultSlot.AddAmount(1);
        }

        RefreshIngredientList();

        Debug.Log("Brewed: " + recipe.result.itemName);
    }

    public void TakeResult() {
        ItemSO potionResult = resultSlot.GetItem();
        int potionAmount = resultSlot.GetAmount();
        inventory.AddItem(potionResult, potionAmount);
        resultSlot.ClearSlot();
    }



    void HighlightIngredientEntryList() {
        for (int i = 0; i < ingredientEntries.Count; i++) {

            Image img = ingredientEntries[i].GetComponent<Image>();
            img.color = (i == selectedIngredientIndex) ? selectedColor : normalColor;
        }
    }

    public void SelectPreviousEntry() {
        if (entryCount == 0) return;
        selectedIngredientIndex = (selectedIngredientIndex - 1 + entryCount) % entryCount;
        HighlightIngredientEntryList();
    }

    public void SelectNextEntry() {
        if (entryCount == 0) return;
        selectedIngredientIndex = (selectedIngredientIndex + 1) % entryCount;
        HighlightIngredientEntryList();
    }


}
