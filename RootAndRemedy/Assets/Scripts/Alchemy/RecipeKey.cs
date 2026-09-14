using System;

public class RecipeKey : IEquatable<RecipeKey> {
    private readonly ItemSO ingredientOne;
    private readonly ItemSO ingredientTwo;

    public RecipeKey(ItemSO ingredientOne, ItemSO ingredientTwo) {
        this.ingredientOne = ingredientOne;
        this.ingredientTwo = ingredientTwo;
    }

    public bool Equals(RecipeKey other) {
        if (other == null)
            return false;

        bool sameOrder =
            ingredientOne == other.ingredientOne &&
            ingredientTwo == other.ingredientTwo;

        bool reverseOrder =
            ingredientOne == other.ingredientTwo &&
            ingredientTwo == other.ingredientOne;

        return sameOrder || reverseOrder;
    }

    public override bool Equals(object obj) {
        return Equals(obj as RecipeKey);
    }

    public override int GetHashCode() {
        int hashOne = ingredientOne != null ? ingredientOne.GetHashCode() : 0;
        int hashTwo = ingredientTwo != null ? ingredientTwo.GetHashCode() : 0;

        return hashOne ^ hashTwo;
    }
}
