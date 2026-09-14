using System.Collections.Generic;
using UnityEngine;

public class PotionRecipeDatabase : MonoBehaviour {
    Dictionary<RecipeKey, ItemSO> potionRecipes;

    [SerializeField] ItemSO potionOfFireDraught;
    [SerializeField] ItemSO fireSeed;

    void Awake() {
        potionRecipes = new Dictionary<RecipeKey, ItemSO> {
            { new RecipeKey(fireSeed, fireSeed), potionOfFireDraught }
        };
    }

    public ItemSO GetPotion(ItemSO ingredientOne, ItemSO ingredientTwo) {
        RecipeKey key = new RecipeKey(ingredientOne, ingredientTwo);

        if (potionRecipes.TryGetValue(key, out ItemSO potion)) {
            return potion;
        }

        return null;
    }
}
