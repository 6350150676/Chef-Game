using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Stations
{
    public interface IInteractable
    {
        /// <summary>World position where prompts about this interactable are shown.</summary>
        Vector3 PromptPosition { get; }

        InteractionPrompt GetPrompt(IIngredientHolder holder);

        /// <returns>True if the interaction did something.</returns>
        bool Interact(IIngredientHolder holder);
    }
}
