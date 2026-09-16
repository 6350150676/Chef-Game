using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using YesChef.Core;
using YesChef.Ingredients;
using YesChef.Orders;
using YesChef.Player;
using YesChef.Scoring;
using YesChef.Stations;
using YesChef.UI;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace YesChef.Tests
{
    /// <summary>
    /// End-to-end checks against the real Kitchen scene, so broken scene wiring fails a test instead of a playtest.
    /// </summary>
    public class KitchenFlowTests
    {
        private const string ScenePath = "Assets/_Project/Scenes/Kitchen.unity";

        private GameManager game;
        private PlayerHands hands;

        [UnitySetUp]
        public IEnumerator LoadKitchen()
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#else
            yield return SceneManager.LoadSceneAsync("Kitchen");
#endif
            game = Object.FindAnyObjectByType<GameManager>();
            hands = Object.FindAnyObjectByType<PlayerHands>();
        }

        [TearDown]
        public void RestoreTimeScale() => Time.timeScale = 1f;

        [UnityTest]
        public IEnumerator StartGame_ShowsHudAndOpensAnOrderAtEveryWindow()
        {
            Assert.AreEqual(GameState.MainMenu, game.State);
            Assert.IsNotNull(Object.FindAnyObjectByType<StartPanelView>(), "Start panel should be visible in the main menu.");

            game.StartGame();
            yield return null;

            Assert.AreEqual(GameState.Playing, game.State);
            Assert.IsNull(Object.FindAnyObjectByType<StartPanelView>());
            Assert.IsNotNull(Object.FindAnyObjectByType<HUDView>());

            CustomerWindow[] windows = FindAll<CustomerWindow>();
            Assert.AreEqual(4, windows.Length);
            Assert.That(windows.All(w => w.CurrentOrder != null), "Every window should start with an order.");

            int expectedIcons = windows.Sum(w => w.CurrentOrder.Ingredients.Count);
            Assert.AreEqual(expectedIcons, Object.FindObjectsByType<IngredientIconUI>(FindObjectsSortMode.None).Length);
        }

        [UnityTest]
        public IEnumerator ServingAPreparedOrder_AddsItsScoreAndRespawnsTheOrder()
        {
            game.StartGame();
            yield return null;
            Time.timeScale = 5f;

            var orderManager = Object.FindAnyObjectByType<OrderManager>();
            var scoreManager = Object.FindAnyObjectByType<ScoreManager>();
            int? awardedPoints = null;
            orderManager.OrderCompleted += completion => awardedPoints = completion.Points;

            CustomerWindow window = FindAll<CustomerWindow>()[0];
            foreach (IngredientDefinition ingredient in window.CurrentOrder.Ingredients.ToArray())
            {
                FindAll<RefrigeratorCompartment>().First(c => c.Ingredient == ingredient).Interact(hands);
                Assert.IsTrue(hands.IsHoldingIngredient);

                if (ingredient.RequiresPreparation)
                    yield return Prepare(FindAll<ProcessingStation>().First(s => s.Method == ingredient.RequiredPreparation));

                window.Interact(hands);
                Assert.IsFalse(hands.IsHoldingIngredient, $"{ingredient.DisplayName} should have been served.");
            }

            Assert.IsTrue(awardedPoints.HasValue, "The order should have completed.");
            Assert.AreEqual(awardedPoints.Value, scoreManager.Score);
            Assert.IsNull(window.CurrentOrder);

            yield return WaitFor(() => window.CurrentOrder != null, timeoutSeconds: 5f);
        }

        [UnityTest]
        public IEnumerator Window_RejectsUnpreparedIngredients_WhichStayInHand()
        {
            game.StartGame();
            yield return null;

            IngredientDefinition meat = FindAll<RefrigeratorCompartment>()
                .Select(c => c.Ingredient)
                .First(i => i.RequiredPreparation == PreparationMethod.Cook);
            FindAll<RefrigeratorCompartment>().First(c => c.Ingredient == meat).Interact(hands);

            foreach (CustomerWindow window in FindAll<CustomerWindow>())
                window.Interact(hands);

            Assert.IsTrue(hands.IsHoldingIngredient);

            FindAll<TrashBin>().Single().Interact(hands);
            Assert.IsFalse(hands.IsHoldingIngredient);
        }

        [UnityTest]
        public IEnumerator Prompts_ExplainWhatInteractingWillDo()
        {
            game.StartGame();
            yield return null;

            RefrigeratorCompartment meatFridge = FindAll<RefrigeratorCompartment>()
                .First(c => c.Ingredient.RequiredPreparation == PreparationMethod.Cook);
            ProcessingStation table = FindAll<ProcessingStation>().First(s => s.Method == PreparationMethod.Chop);
            ProcessingStation stove = FindAll<ProcessingStation>().First(s => s.Method == PreparationMethod.Cook);
            CustomerWindow window = FindAll<CustomerWindow>()[0];

            Assert.AreEqual(InteractionKind.TakeIngredient, meatFridge.GetPrompt(hands).Kind);
            meatFridge.Interact(hands);

            Assert.IsFalse(meatFridge.GetPrompt(hands).IsAvailable, "Hands are full.");
            Assert.AreEqual("Needs cooking first", table.GetPrompt(hands).Text);
            Assert.AreEqual("Needs cooking first", window.GetPrompt(hands).Text);
            Assert.AreEqual(InteractionKind.StartPreparing, stove.GetPrompt(hands).Kind);
            Assert.AreEqual("Cook Meat", stove.GetPrompt(hands).Text);
            Assert.AreEqual(InteractionKind.Discard, FindAll<TrashBin>().Single().GetPrompt(hands).Kind);

            Assert.IsFalse(table.Interact(hands), "Rejected interactions report failure.");
            Assert.IsTrue(hands.IsHoldingIngredient);
        }

        [UnityTest]
        public IEnumerator Pause_FreezesTheRoundClockUntilResumed()
        {
            game.StartGame();
            yield return null;

            game.PauseGame();
            Assert.AreEqual(GameState.Paused, game.State);
            Assert.IsNotNull(Object.FindAnyObjectByType<PausePanelView>());

            float remaining = game.TimeRemaining;
            yield return null;
            yield return null;
            Assert.AreEqual(remaining, game.TimeRemaining);

            game.ResumeGame();
            Assert.AreEqual(GameState.Playing, game.State);
            Assert.AreEqual(1f, Time.timeScale);
        }

        private IEnumerator Prepare(ProcessingStation station)
        {
            // Stand in for the chef being at the station; the table only chops while attended.
            station.SetFocused(true);
            station.Interact(hands);
            Assert.IsFalse(hands.IsHoldingIngredient, $"{station.name} should have taken the ingredient.");

            yield return WaitFor(() => Enumerable.Range(0, station.SlotCount).Any(i => station.GetSlot(i).IsDone), 10f);

            station.Interact(hands);
            station.SetFocused(false);
            Assert.IsTrue(hands.HeldIngredient.IsPrepared);
        }

        private static T[] FindAll<T>() where T : Object
        {
            return Object.FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
        }

        private static IEnumerator WaitFor(Func<bool> condition, float timeoutSeconds)
        {
            float deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (!condition())
            {
                if (Time.realtimeSinceStartup > deadline)
                    Assert.Fail($"Condition not met within {timeoutSeconds} seconds.");
                yield return null;
            }
        }
    }
}
