using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(
    fileName = "New Potion Recipe",
    menuName = "Potion Recipe"
)]
public class PotionRecipeSO : ScriptableObject {
    public List<RecipeIngredient> ingredients = new();
    public ItemSO result;
}
