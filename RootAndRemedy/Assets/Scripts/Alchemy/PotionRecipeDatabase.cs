using System.Collections.Generic;
using UnityEngine;

public class PotionRecipeDatabase : MonoBehaviour {
    //Dictionary<RecipeKey, ItemSO> potionRecipes;
    [SerializeField] private List<PotionRecipeSO> recipes = new();
    public PotionRecipeSO GetRecipe(List<ItemSO> selectedIngredients) {
        foreach (PotionRecipeSO recipe in recipes) {
            if (RecipeMatches(recipe, selectedIngredients))
                return recipe;
        }

        return null;
    }
    private bool RecipeMatches(PotionRecipeSO recipe, List<ItemSO> selectedIngredients) {
        if (recipe.ingredients.Count != selectedIngredients.Count)
            return false;

        foreach (RecipeIngredient ingredient in recipe.ingredients) {
            if (!selectedIngredients.Contains(ingredient.item))
                return false;
        }

        return true;
    }

    /*     [SerializeField] ItemSO potionOfFireDraught;
        [SerializeField] ItemSO fireBloom;

        void Awake() {
            potionRecipes = new Dictionary<RecipeKey, ItemSO> {
                { new RecipeKey(fireBloom, fireBloom), potionOfFireDraught }
            };
        }

        public ItemSO GetPotion(ItemSO ingredientOne, ItemSO ingredientTwo) {
            RecipeKey key = new RecipeKey(ingredientOne, ingredientTwo);

            if (potionRecipes.TryGetValue(key, out ItemSO potion)) {
                return potion;
            }

            return null;
        } */
}
