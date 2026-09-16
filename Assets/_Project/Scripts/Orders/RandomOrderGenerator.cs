using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Orders
{
    /// <summary>
    /// Picks an order size uniformly between the min and max (2 or 3 by default, i.e. 50/50), then fills it with
    /// random ingredients from the pool. Duplicates are allowed.
    /// </summary>
    [CreateAssetMenu(menuName = "Yes Chef/Order Generators/Random", fileName = "RandomOrderGenerator")]
    public sealed class RandomOrderGenerator : OrderGenerator
    {
        [SerializeField] private IngredientDefinition[] ingredientPool;
        [SerializeField, Min(1)] private int minIngredients = 2;
        [SerializeField, Min(1)] private int maxIngredients = 3;

        public override Order CreateOrder()
        {
            if (ingredientPool == null || ingredientPool.Length == 0)
                throw new System.InvalidOperationException($"{name} has an empty ingredient pool.");

            int count = Random.Range(minIngredients, maxIngredients + 1);
            var picks = new IngredientDefinition[count];
            for (int i = 0; i < count; i++)
                picks[i] = ingredientPool[Random.Range(0, ingredientPool.Length)];

            return new Order(picks);
        }

        private void OnValidate() => maxIngredients = Mathf.Max(minIngredients, maxIngredients);
    }
}
