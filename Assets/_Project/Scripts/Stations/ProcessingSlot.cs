using UnityEngine;
using YesChef.Core;
using YesChef.Ingredients;

namespace YesChef.Stations
{
    /// <summary>One spot on a station (a chopping board, a burner) that prepares a single ingredient over time.</summary>
    public sealed class ProcessingSlot
    {
        private readonly Transform anchor;
        private readonly CountdownTimer timer = new CountdownTimer();

        public ProcessingSlot(Transform anchor) => this.anchor = anchor;

        public Ingredient CurrentIngredient { get; private set; }
        public bool IsEmpty => CurrentIngredient == null;
        public bool IsProcessing => !IsEmpty && !CurrentIngredient.IsPrepared;
        public bool IsDone => !IsEmpty && CurrentIngredient.IsPrepared;
        public float Progress01 => timer.Progress01;
        public float RemainingSeconds => timer.Remaining;
        public Vector3 Position => anchor.position;

        public void Place(Ingredient ingredient, float durationSeconds)
        {
            CurrentIngredient = ingredient;
            ingredient.AttachTo(anchor);
            timer.Start(durationSeconds);

            if (!timer.IsRunning)
                ingredient.CompletePreparation();
        }

        /// <returns>True on the tick the ingredient finishes preparing.</returns>
        public bool Tick(float deltaTime)
        {
            if (!IsProcessing || !timer.Tick(deltaTime))
                return false;

            CurrentIngredient.CompletePreparation();
            return true;
        }

        public Ingredient Take()
        {
            Ingredient taken = CurrentIngredient;
            CurrentIngredient = null;
            return taken;
        }

        public void Clear()
        {
            if (!IsEmpty)
                CurrentIngredient.Discard();

            CurrentIngredient = null;
            timer.Stop();
        }
    }
}
