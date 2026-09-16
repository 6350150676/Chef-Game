using System;
using System.Collections.Generic;
using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Orders
{
    /// <summary>
    /// A customer's request: the ingredients still needed and how long the customer has been waiting.
    /// Pure C# so the rules are unit-testable without a scene.
    /// </summary>
    public sealed class Order
    {
        private readonly IngredientDefinition[] ingredients;
        private readonly bool[] delivered;
        private int pendingCount;

        public Order(IReadOnlyList<IngredientDefinition> requiredIngredients)
        {
            if (requiredIngredients == null || requiredIngredients.Count == 0)
                throw new ArgumentException("An order needs at least one ingredient.", nameof(requiredIngredients));

            ingredients = new IngredientDefinition[requiredIngredients.Count];
            for (int i = 0; i < ingredients.Length; i++)
            {
                ingredients[i] = requiredIngredients[i];
                BaseValue += ingredients[i].ScoreValue;
            }

            delivered = new bool[ingredients.Length];
            pendingCount = ingredients.Length;
        }

        /// <summary>Raised whenever an ingredient is delivered.</summary>
        public event Action<Order> Changed;

        public IReadOnlyList<IngredientDefinition> Ingredients => ingredients;
        public int BaseValue { get; }
        public float ElapsedSeconds { get; private set; }
        public bool IsComplete => pendingCount == 0;

        public bool IsDelivered(int index) => delivered[index];

        public bool Accepts(IngredientDefinition ingredient) => IndexOfPending(ingredient) >= 0;

        public void Tick(float deltaTime)
        {
            if (!IsComplete)
                ElapsedSeconds += deltaTime;
        }

        public bool TryDeliver(IngredientDefinition ingredient)
        {
            int index = IndexOfPending(ingredient);
            if (index < 0)
                return false;

            delivered[index] = true;
            pendingCount--;
            Changed?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Sum of ingredient values minus whole seconds waited (14.99s costs 14 points). Can go negative.
        /// </summary>
        public int CalculateScore() => BaseValue - Mathf.FloorToInt(ElapsedSeconds);

        private int IndexOfPending(IngredientDefinition ingredient)
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                if (!delivered[i] && ingredients[i] == ingredient)
                    return i;
            }

            return -1;
        }
    }
}
