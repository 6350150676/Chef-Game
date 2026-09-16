using System;
using UnityEngine;
using YesChef.Core;
using YesChef.Ingredients;

namespace YesChef.Orders
{
    /// <summary>
    /// Runs the order lifecycle at every customer window: fills all windows when a round starts, ages open orders,
    /// scores completed ones and respawns a new order after the configured delay.
    /// </summary>
    public sealed class OrderManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameConfig config;
        [SerializeField] private OrderGenerator orderGenerator;
        [SerializeField] private CustomerWindow[] windows;

        private CountdownTimer[] respawnTimers;

        /// <summary>(windowIndex, order)</summary>
        public event Action<int, Order> OrderStarted;
        public event Action<OrderCompletion> OrderCompleted;

        public int WindowCount => windows.Length;

        public Order GetOrder(int windowIndex) => windows[windowIndex].CurrentOrder;

        /// <summary>True if any open order still needs this ingredient.</summary>
        public bool IsWantedByAnyOrder(IngredientDefinition ingredient)
        {
            foreach (CustomerWindow window in windows)
            {
                if (window.CurrentOrder != null && window.CurrentOrder.Accepts(ingredient))
                    return true;
            }

            return false;
        }

        private void Awake()
        {
            respawnTimers = new CountdownTimer[windows.Length];
            for (int i = 0; i < respawnTimers.Length; i++)
                respawnTimers[i] = new CountdownTimer();
        }

        private void OnEnable()
        {
            gameManager.RoundStarted += HandleRoundStarted;
            foreach (CustomerWindow window in windows)
                window.OrderFulfilled += HandleOrderFulfilled;
        }

        private void OnDisable()
        {
            gameManager.RoundStarted -= HandleRoundStarted;
            foreach (CustomerWindow window in windows)
                window.OrderFulfilled -= HandleOrderFulfilled;
        }

        private void Update()
        {
            if (!gameManager.IsPlaying)
                return;

            float deltaTime = Time.deltaTime;
            for (int i = 0; i < windows.Length; i++)
            {
                Order order = windows[i].CurrentOrder;
                if (order != null)
                {
                    order.Tick(deltaTime);
                    continue;
                }

                respawnTimers[i].Tick(deltaTime);
                if (!respawnTimers[i].IsRunning)
                    SpawnOrder(i);
            }
        }

        private void HandleRoundStarted()
        {
            for (int i = 0; i < windows.Length; i++)
            {
                respawnTimers[i].Stop();
                SpawnOrder(i);
            }
        }

        private void SpawnOrder(int windowIndex)
        {
            Order order = orderGenerator.CreateOrder();
            windows[windowIndex].AssignOrder(order);
            OrderStarted?.Invoke(windowIndex, order);
        }

        private void HandleOrderFulfilled(CustomerWindow window)
        {
            int windowIndex = Array.IndexOf(windows, window);
            Order order = window.CurrentOrder;
            int points = order.CalculateScore();

            window.ClearOrder();
            respawnTimers[windowIndex].Start(config.OrderRespawnDelaySeconds);
            OrderCompleted?.Invoke(new OrderCompletion(windowIndex, order, points));
        }
    }
}
