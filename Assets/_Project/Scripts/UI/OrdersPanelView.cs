using UnityEngine;
using YesChef.Orders;

namespace YesChef.UI
{
    /// <summary>Routes order events to the <see cref="OrderUI"/> that belongs to each customer window.</summary>
    public sealed class OrdersPanelView : MonoBehaviour
    {
        [SerializeField] private OrderManager orderManager;

        [Tooltip("Element i displays customer window i.")]
        [SerializeField] private OrderUI[] orderUIs;

        private void OnEnable()
        {
            orderManager.OrderStarted += HandleOrderStarted;
            orderManager.OrderCompleted += HandleOrderCompleted;

            int count = Mathf.Min(orderUIs.Length, orderManager.WindowCount);
            for (int i = 0; i < count; i++)
                orderUIs[i].Bind(orderManager.GetOrder(i));
        }

        private void OnDisable()
        {
            orderManager.OrderStarted -= HandleOrderStarted;
            orderManager.OrderCompleted -= HandleOrderCompleted;
        }

        private void HandleOrderStarted(int windowIndex, Order order) => orderUIs[windowIndex].Bind(order);

        private void HandleOrderCompleted(OrderCompletion completion)
        {
            OrderUI orderUI = orderUIs[completion.WindowIndex];
            orderUI.Bind(null);
            orderUI.ShowScorePopup(completion.Points);
        }
    }
}
