using System;
using YesChef.Ingredients;
using YesChef.Stations;

namespace YesChef.Orders
{
    /// <summary>
    /// Serving hatch that accepts ingredients for the order currently assigned to it.
    /// Order lifecycle (spawning, timing, scoring) belongs to <see cref="OrderManager"/>.
    /// </summary>
    public sealed class CustomerWindow : InteractableStation
    {
        public Order CurrentOrder { get; private set; }

        public event Action<Order> OrderAssigned;
        public event Action<IngredientDefinition> IngredientServed;
        public event Action<CustomerWindow> OrderFulfilled;

        public void AssignOrder(Order order)
        {
            CurrentOrder = order;
            OrderAssigned?.Invoke(order);
        }

        public void ClearOrder() => CurrentOrder = null;

        protected override InteractionPrompt Evaluate(IIngredientHolder holder)
        {
            if (CurrentOrder == null)
                return InteractionPrompt.Blocked("No customer yet");

            if (!holder.IsHoldingIngredient)
                return InteractionPrompt.Blocked("Bring what they ordered");

            // Anything the order can't take stays in the chef's hands.
            Ingredient ingredient = holder.HeldIngredient;
            if (!ingredient.IsReadyToServe)
                return InteractionPrompt.Blocked(ingredient.Definition.RequiredPreparation.NeedsFirst());

            if (!CurrentOrder.Accepts(ingredient.Definition))
                return InteractionPrompt.Blocked("Not on this order");

            return InteractionPrompt.Available(InteractionKind.Serve, $"Serve {ingredient.DisplayName}");
        }

        protected override void Perform(InteractionKind kind, IIngredientHolder holder)
        {
            Order order = CurrentOrder;
            IngredientDefinition served = holder.HeldIngredient.Definition;

            order.TryDeliver(served);
            holder.ReleaseIngredient().Discard();
            IngredientServed?.Invoke(served);

            if (order.IsComplete)
                OrderFulfilled?.Invoke(this);
        }
    }
}
