using UnityEngine;

namespace YesChef.Ingredients
{
    /// <summary>
    /// Data for one kind of ingredient. Adding a new ingredient is a new asset and prefab, not new code.
    /// </summary>
    [CreateAssetMenu(menuName = "Yes Chef/Ingredient", fileName = "Ingredient")]
    public sealed class IngredientDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Ingredient";
        [SerializeField] private int scoreValue = 10;
        [SerializeField] private PreparationMethod requiredPreparation = PreparationMethod.None;

        [Header("Presentation")]
        [Tooltip("Shown on order tickets.")]
        [SerializeField] private Sprite icon;
        [SerializeField] private Color uiColor = Color.white;
        [Tooltip("Prefab with this ingredient's raw and prepared visuals in the kitchen.")]
        [SerializeField] private Ingredient prefab;

        public string DisplayName => displayName;
        public int ScoreValue => scoreValue;
        public PreparationMethod RequiredPreparation => requiredPreparation;
        public bool RequiresPreparation => requiredPreparation != PreparationMethod.None;
        public Sprite Icon => icon;
        public Color UIColor => uiColor;
        public Ingredient Prefab => prefab;
    }
}
