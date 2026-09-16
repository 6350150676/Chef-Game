using System;
using UnityEngine;
using YesChef.Core;
using YesChef.Ingredients;

namespace YesChef.Stations
{
    /// <summary>
    /// A station that prepares ingredients with one <see cref="PreparationMethod"/>. The chopping table and the stove
    /// are both this component with different data: method, slot count, duration and whether the chef must stay.
    /// </summary>
    public sealed class ProcessingStation : InteractableStation
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PreparationMethod method = PreparationMethod.Chop;
        [SerializeField, Min(0f)] private float processDurationSeconds = 2f;

        [Tooltip("If enabled, progress only advances while the chef is at this station (active work such as chopping). " +
                 "If disabled, ingredients prepare unattended (such as cooking).")]
        [SerializeField] private bool requiresAttendance;

        [Tooltip("One anchor per slot. The number of anchors is the station's capacity.")]
        [SerializeField] private Transform[] slotAnchors;

        private ProcessingSlot[] slots;
        private string idleText;
        private string workingText;
        private string fullText;

        /// <summary>(station, slotIndex) raised when an ingredient finishes preparing.</summary>
        public event Action<ProcessingStation, int> SlotCompleted;

        public PreparationMethod Method => method;
        public int SlotCount => slotAnchors.Length;

        /// <summary>True while an attended station is waiting for the chef to come back.</summary>
        public bool IsStalled => requiresAttendance && !IsFocused;

        public ProcessingSlot GetSlot(int index) => slots[index];

        private void Awake()
        {
            slots = new ProcessingSlot[slotAnchors.Length];
            for (int i = 0; i < slots.Length; i++)
                slots[i] = new ProcessingSlot(slotAnchors[i]);

            idleText = $"Bring something to {method.Verb().ToLowerInvariant()}";
            workingText = requiresAttendance ? $"{method.InProgress()}... stay close" : $"{method.InProgress()}...";
            fullText = $"{DisplayName} is full";
        }

        private void OnEnable() => gameManager.RoundStarted += ClearSlots;

        private void OnDisable() => gameManager.RoundStarted -= ClearSlots;

        private void Update()
        {
            if (!gameManager.IsPlaying || IsStalled)
                return;

            float deltaTime = Time.deltaTime;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Tick(deltaTime))
                    SlotCompleted?.Invoke(this, i);
            }
        }

        protected override InteractionPrompt Evaluate(IIngredientHolder holder)
        {
            if (holder.IsHoldingIngredient)
                return EvaluatePlacing(holder.HeldIngredient);

            int finished = IndexOfFinishedSlot();
            if (finished >= 0)
                return InteractionPrompt.Available(InteractionKind.CollectPrepared,
                    $"Pick up {slots[finished].CurrentIngredient.DisplayName}");

            return InteractionPrompt.Blocked(IsAnySlotProcessing() ? workingText : idleText);
        }

        protected override void Perform(InteractionKind kind, IIngredientHolder holder)
        {
            if (kind == InteractionKind.StartPreparing)
            {
                int index = IndexOfEmptySlot();
                slots[index].Place(holder.ReleaseIngredient(), processDurationSeconds);

                if (slots[index].IsDone)
                    SlotCompleted?.Invoke(this, index);
            }
            else if (kind == InteractionKind.CollectPrepared)
            {
                holder.TryHold(slots[IndexOfFinishedSlot()].Take());
            }
        }

        private InteractionPrompt EvaluatePlacing(Ingredient ingredient)
        {
            if (ingredient.IsReadyToServe)
                return InteractionPrompt.Blocked("Ready to serve");

            if (!ingredient.CanBePreparedWith(method))
                return InteractionPrompt.Blocked(ingredient.Definition.RequiredPreparation.NeedsFirst());

            if (IndexOfEmptySlot() < 0)
                return InteractionPrompt.Blocked(fullText);

            return InteractionPrompt.Available(InteractionKind.StartPreparing, $"{method.Verb()} {ingredient.Definition.DisplayName}");
        }

        private int IndexOfEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty)
                    return i;
            }

            return -1;
        }

        private int IndexOfFinishedSlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsDone)
                    return i;
            }

            return -1;
        }

        private bool IsAnySlotProcessing()
        {
            foreach (ProcessingSlot slot in slots)
            {
                if (slot.IsProcessing)
                    return true;
            }

            return false;
        }

        private void ClearSlots()
        {
            foreach (ProcessingSlot slot in slots)
                slot.Clear();
        }
    }
}
