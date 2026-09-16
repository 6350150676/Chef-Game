using YesChef.Ingredients;

namespace YesChef.Stations
{
    public sealed class TrashBin : InteractableStation
    {
        protected override InteractionPrompt Evaluate(IIngredientHolder holder)
        {
            return holder.IsHoldingIngredient
                ? InteractionPrompt.Available(InteractionKind.Discard, $"Throw away {holder.HeldIngredient.DisplayName}")
                : InteractionPrompt.Blocked("Nothing to throw away");
        }

        protected override void Perform(InteractionKind kind, IIngredientHolder holder)
        {
            holder.ReleaseIngredient().Discard();
        }
    }
}
