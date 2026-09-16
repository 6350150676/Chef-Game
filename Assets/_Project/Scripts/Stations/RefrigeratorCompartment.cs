using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Stations
{
    /// <summary>
    /// One door of the refrigerator, holding an endless supply of a single raw ingredient.
    /// The refrigerator is a row of these, so stocking a new ingredient is just another compartment.
    /// </summary>
    public sealed class RefrigeratorCompartment : InteractableStation
    {
        [SerializeField] private IngredientDefinition ingredient;
        [SerializeField] private IngredientFactory ingredientFactory;

        private string takeText;

        public IngredientDefinition Ingredient => ingredient;

        private void Awake() => takeText = $"Take {ingredient.DisplayName}";

        protected override InteractionPrompt Evaluate(IIngredientHolder holder)
        {
            return holder.IsHoldingIngredient
                ? InteractionPrompt.Blocked("Hands full")
                : InteractionPrompt.Available(InteractionKind.TakeIngredient, takeText);
        }

        protected override void Perform(InteractionKind kind, IIngredientHolder holder)
        {
            holder.TryHold(ingredientFactory.Create(ingredient));
        }
    }
}
