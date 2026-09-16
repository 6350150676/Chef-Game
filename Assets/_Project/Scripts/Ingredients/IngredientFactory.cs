using UnityEngine;

namespace YesChef.Ingredients
{
    /// <summary>
    /// The one place ingredient instances are built. Pooling later only touches this class and <see cref="Ingredient.Discard"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "Yes Chef/Ingredient Factory", fileName = "IngredientFactory")]
    public sealed class IngredientFactory : ScriptableObject
    {
        public Ingredient Create(IngredientDefinition definition)
        {
            Ingredient ingredient = Instantiate(definition.Prefab);
            ingredient.Initialize(definition);
            return ingredient;
        }
    }
}
