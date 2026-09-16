using UnityEngine;

namespace YesChef.Ingredients
{
    /// <summary>
    /// A physical ingredient in the kitchen that can be carried, placed on stations and served.
    /// </summary>
    public sealed class Ingredient : MonoBehaviour
    {
        [SerializeField] private GameObject rawVisual;
        [Tooltip("Optional. Ingredients served as they are don't need one.")]
        [SerializeField] private GameObject preparedVisual;

        public IngredientDefinition Definition { get; private set; }
        public bool IsPrepared { get; private set; }

        /// <summary>Name including its state, e.g. "Raw Meat" or "Chopped Veggie".</summary>
        public string DisplayName { get; private set; }

        /// <summary>True once prepared, or immediately for ingredients that need no preparation.</summary>
        public bool IsReadyToServe => IsPrepared || !Definition.RequiresPreparation;

        public void Initialize(IngredientDefinition definition)
        {
            Definition = definition;
            IsPrepared = false;
            name = definition.DisplayName;
            RefreshState();
        }

        public bool CanBePreparedWith(PreparationMethod method)
        {
            return !IsPrepared && method != PreparationMethod.None && Definition.RequiredPreparation == method;
        }

        public void CompletePreparation()
        {
            IsPrepared = true;
            RefreshState();
        }

        public void AttachTo(Transform anchor)
        {
            transform.SetParent(anchor, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Discard() => Destroy(gameObject);

        private void RefreshState()
        {
            if (IsPrepared)
                DisplayName = $"{Definition.RequiredPreparation.Done()} {Definition.DisplayName}";
            else
                DisplayName = Definition.RequiresPreparation ? $"Raw {Definition.DisplayName}" : Definition.DisplayName;

            bool showPrepared = IsPrepared && preparedVisual != null;
            rawVisual.SetActive(!showPrepared);
            if (preparedVisual != null)
                preparedVisual.SetActive(showPrepared);
        }
    }
}
