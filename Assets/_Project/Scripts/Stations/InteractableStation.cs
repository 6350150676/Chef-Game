using System;
using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Stations
{
    /// <summary>
    /// Base for every kitchen station. <see cref="Interact"/> is a template method: evaluate what would happen,
    /// perform it only if allowed, then report the outcome so presentation can react.
    /// </summary>
    public abstract class InteractableStation : MonoBehaviour, IInteractable, IFocusable
    {
        [SerializeField] private string displayName = "Station";

        [Tooltip("Where prompts and feedback appear. Defaults to the station's own position.")]
        [SerializeField] private Transform promptAnchor;

        public event Action<bool> FocusChanged;

        /// <summary>Raised after every interaction attempt with the prompt that applied. Unavailable means it was rejected.</summary>
        public event Action<InteractionPrompt> Interacted;

        public string DisplayName => displayName;
        public bool IsFocused { get; private set; }
        public Vector3 PromptPosition => (promptAnchor != null ? promptAnchor : transform).position;

        public void SetFocused(bool isFocused)
        {
            if (IsFocused == isFocused)
                return;

            IsFocused = isFocused;
            FocusChanged?.Invoke(isFocused);
        }

        public InteractionPrompt GetPrompt(IIngredientHolder holder) => Evaluate(holder);

        public bool Interact(IIngredientHolder holder)
        {
            InteractionPrompt prompt = Evaluate(holder);
            if (prompt.IsAvailable)
                Perform(prompt.Kind, holder);

            Interacted?.Invoke(prompt);
            return prompt.IsAvailable;
        }

        protected abstract InteractionPrompt Evaluate(IIngredientHolder holder);

        /// <summary>Carries out an interaction that <see cref="Evaluate"/> has just reported as available.</summary>
        protected abstract void Perform(InteractionKind kind, IIngredientHolder holder);
    }
}
