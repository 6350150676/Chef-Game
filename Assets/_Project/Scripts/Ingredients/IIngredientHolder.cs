namespace YesChef.Ingredients
{
    /// <summary>
    /// Anything that can carry one ingredient. Stations depend on this rather than on the player,
    /// so other actors (helpers, NPCs) could use them too.
    /// </summary>
    public interface IIngredientHolder
    {
        Ingredient HeldIngredient { get; }
        bool IsHoldingIngredient { get; }

        /// <returns>False if already holding something.</returns>
        bool TryHold(Ingredient ingredient);

        /// <summary>Gives up the held ingredient (possibly null). The caller becomes its owner.</summary>
        Ingredient ReleaseIngredient();
    }
}
