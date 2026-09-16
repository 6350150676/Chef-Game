using System;
using NUnit.Framework;
using YesChef.Ingredients;
using YesChef.Orders;

namespace YesChef.Tests
{
    public class OrderTests
    {
        private TestIngredients ingredients;
        private IngredientDefinition vegetable;
        private IngredientDefinition cheese;
        private IngredientDefinition meat;

        [SetUp]
        public void SetUp()
        {
            ingredients = new TestIngredients();
            vegetable = ingredients.Create("Veggie", 20, PreparationMethod.Chop);
            cheese = ingredients.Create("Cheese", 10);
            meat = ingredients.Create("Meat", 30, PreparationMethod.Cook);
        }

        [TearDown]
        public void TearDown() => ingredients.DestroyAll();

        [Test]
        public void CalculateScore_SubtractsWholeSecondsElapsed()
        {
            // The spec's example: cheese + meat delivered in 14.99 seconds scores 10 + 30 - 14.
            var order = new Order(new[] { cheese, meat });
            order.Tick(14.99f);

            Assert.AreEqual(26, order.CalculateScore());
        }

        [Test]
        public void CalculateScore_CanGoNegative()
        {
            var order = new Order(new[] { cheese, cheese });
            order.Tick(26.5f);

            Assert.AreEqual(-6, order.CalculateScore());
        }

        [Test]
        public void TryDeliver_RejectsIngredientsTheOrderDoesNotNeed()
        {
            var order = new Order(new[] { cheese, meat });

            Assert.IsFalse(order.TryDeliver(vegetable));
            Assert.IsFalse(order.IsComplete);
        }

        [Test]
        public void TryDeliver_AcceptsDuplicatesOnlyAsOftenAsRequired()
        {
            var order = new Order(new[] { meat, meat, cheese });

            Assert.IsTrue(order.TryDeliver(meat));
            Assert.IsTrue(order.TryDeliver(meat));
            Assert.IsFalse(order.TryDeliver(meat));
            Assert.IsFalse(order.IsComplete);

            Assert.IsTrue(order.TryDeliver(cheese));
            Assert.IsTrue(order.IsComplete);
        }

        [Test]
        public void TryDeliver_MarksTheMatchingIngredientAndRaisesChanged()
        {
            var order = new Order(new[] { cheese, vegetable });
            int changedCount = 0;
            order.Changed += _ => changedCount++;

            order.TryDeliver(vegetable);

            Assert.IsFalse(order.IsDelivered(0));
            Assert.IsTrue(order.IsDelivered(1));
            Assert.AreEqual(1, changedCount);
        }

        [Test]
        public void Tick_StopsAgingOnceComplete()
        {
            var order = new Order(new[] { cheese });
            order.Tick(3f);
            order.TryDeliver(cheese);
            order.Tick(10f);

            Assert.AreEqual(7, order.CalculateScore());
        }

        [Test]
        public void Constructor_RejectsEmptyOrders()
        {
            Assert.Throws<ArgumentException>(() => new Order(Array.Empty<IngredientDefinition>()));
        }
    }
}
