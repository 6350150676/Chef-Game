using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Tests
{
    /// <summary>Creates throwaway ingredient assets for tests and cleans them up afterwards.</summary>
    internal sealed class TestIngredients
    {
        private readonly List<Object> created = new List<Object>();

        public IngredientDefinition Create(string displayName, int scoreValue, PreparationMethod preparation = PreparationMethod.None)
        {
            var ingredient = ScriptableObject.CreateInstance<IngredientDefinition>();
            var serialized = new SerializedObject(ingredient);
            serialized.FindProperty("displayName").stringValue = displayName;
            serialized.FindProperty("scoreValue").intValue = scoreValue;
            serialized.FindProperty("requiredPreparation").enumValueIndex = (int)preparation;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            created.Add(ingredient);
            return ingredient;
        }

        public T Track<T>(T asset) where T : Object
        {
            created.Add(asset);
            return asset;
        }

        public void DestroyAll()
        {
            foreach (Object asset in created)
                Object.DestroyImmediate(asset);
            created.Clear();
        }
    }
}
