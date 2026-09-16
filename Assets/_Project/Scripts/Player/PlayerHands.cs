using System;
using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Player
{
    /// <summary>The chef's hands: holds at most one ingredient at the hold point.</summary>
    public sealed class PlayerHands : MonoBehaviour, IIngredientHolder
    {
        [SerializeField] private Transform holdPoint;

        /// <summary>Raised with the new held ingredient, or null when the hands become empty.</summary>
        public event Action<Ingredient> HeldIngredientChanged;

        public Ingredient HeldIngredient { get; private set; }
        public bool IsHoldingIngredient => HeldIngredient != null;

        public bool TryHold(Ingredient ingredient)
        {
            if (IsHoldingIngredient || ingredient == null)
                return false;

            HeldIngredient = ingredient;
            ingredient.AttachTo(holdPoint);
            HeldIngredientChanged?.Invoke(ingredient);
            return true;
        }

        public Ingredient ReleaseIngredient()
        {
            Ingredient released = HeldIngredient;
            if (released == null)
                return null;

            HeldIngredient = null;
            released.transform.SetParent(null);
            HeldIngredientChanged?.Invoke(null);
            return released;
        }

        public void DiscardHeldIngredient()
        {
            if (IsHoldingIngredient)
                ReleaseIngredient().Discard();
        }
    }
}
