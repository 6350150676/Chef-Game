using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using YesChef.Ingredients;
using YesChef.Orders;

namespace YesChef.Tests
{
    public class RandomOrderGeneratorTests
    {
        private TestIngredients ingredients;

        [SetUp]
        public void SetUp() => ingredients = new TestIngredients();

        [TearDown]
        public void TearDown() => ingredients.DestroyAll();

        [Test]
        public void CreateOrder_MakesTwoOrThreeIngredientOrdersFromThePool()
        {
            IngredientDefinition[] pool =
            {
                ingredients.Create("Veggie", 20, PreparationMethod.Chop),
                ingredients.Create("Cheese", 10),
                ingredients.Create("Meat", 30, PreparationMethod.Cook)
            };
            RandomOrderGenerator generator = CreateGenerator(pool);
            Random.InitState(1234);

            var sizesSeen = new HashSet<int>();
            for (int i = 0; i < 200; i++)
            {
                Order order = generator.CreateOrder();
                sizesSeen.Add(order.Ingredients.Count);
                foreach (IngredientDefinition ingredient in order.Ingredients)
                    CollectionAssert.Contains(pool, ingredient);
            }

            CollectionAssert.AreEquivalent(new[] { 2, 3 }, sizesSeen);
        }

        private RandomOrderGenerator CreateGenerator(IngredientDefinition[] pool)
        {
            RandomOrderGenerator generator = ingredients.Track(ScriptableObject.CreateInstance<RandomOrderGenerator>());
            var serialized = new SerializedObject(generator);
            SerializedProperty poolProperty = serialized.FindProperty("ingredientPool");
            poolProperty.arraySize = pool.Length;
            for (int i = 0; i < pool.Length; i++)
                poolProperty.GetArrayElementAtIndex(i).objectReferenceValue = pool[i];
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return generator;
        }
    }
}
