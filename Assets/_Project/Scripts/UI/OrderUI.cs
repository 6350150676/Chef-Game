using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Ingredients;
using YesChef.Orders;

namespace YesChef.UI
{
    /// <summary>
    /// Order ticket for one customer window: the ingredient checklist, how long the order has been open,
    /// how many points it is still worth, and the score popup when it completes.
    /// </summary>
    public sealed class OrderUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup ticket;
        [SerializeField] private UIPopIn ticketPopIn;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private Image patienceFill;
        [SerializeField] private Transform ingredientsContainer;
        [SerializeField] private IngredientIconUI ingredientIconPrefab;
        [SerializeField] private ScorePopupUI scorePopup;

        [Header("Urgency")]
        [Tooltip("Evaluated by how much of the order's value the wait has used up (0 = fresh, 1 = worth nothing).")]
        [SerializeField] private Gradient urgencyColors = new Gradient();
        [SerializeField] private Color valueColor = Color.black;
        [SerializeField] private Color negativeValueColor = new Color(0.85f, 0.2f, 0.15f);

        private readonly List<IngredientIconUI> icons = new List<IngredientIconUI>();
        private Order order;
        private int displayedSeconds = -1;

        public void Bind(Order newOrder)
        {
            Unbind();
            order = newOrder;
            SetTicketVisible(order != null);

            if (order == null)
                return;

            order.Changed += RefreshDeliveredStates;
            PopulateIngredients();
            displayedSeconds = -1;
            Refresh();
            ticketPopIn.Play();
        }

        public void ShowScorePopup(int points) => scorePopup.Play(points);

        private void OnDisable() => Unbind();

        private void Update()
        {
            if (order != null)
                Refresh();
        }

        private void Unbind()
        {
            if (order != null)
                order.Changed -= RefreshDeliveredStates;

            order = null;
        }

        private void PopulateIngredients()
        {
            IReadOnlyList<IngredientDefinition> ingredients = order.Ingredients;

            // Reuse icons between orders; only instantiate when an order is bigger than any before it.
            while (icons.Count < ingredients.Count)
                icons.Add(Instantiate(ingredientIconPrefab, ingredientsContainer));

            for (int i = 0; i < icons.Count; i++)
            {
                bool isUsed = i < ingredients.Count;
                icons[i].gameObject.SetActive(isUsed);
                if (isUsed)
                    icons[i].Show(ingredients[i], order.IsDelivered(i));
            }
        }

        private void RefreshDeliveredStates(Order changedOrder)
        {
            for (int i = 0; i < changedOrder.Ingredients.Count; i++)
                icons[i].SetDelivered(changedOrder.IsDelivered(i));
        }

        private void Refresh()
        {
            float valueUsed01 = Mathf.Clamp01(order.ElapsedSeconds / Mathf.Max(1, order.BaseValue));
            patienceFill.rectTransform.anchorMax = new Vector2(1f - valueUsed01, 1f);
            patienceFill.color = urgencyColors.Evaluate(valueUsed01);

            int seconds = Mathf.FloorToInt(order.ElapsedSeconds);
            if (seconds == displayedSeconds)
                return;

            displayedSeconds = seconds;
            timerText.text = $"{seconds / 60}:{seconds % 60:00}";

            int value = order.CalculateScore();
            valueText.text = value.ToString();
            valueText.color = value < 0 ? negativeValueColor : valueColor;
        }

        private void SetTicketVisible(bool visible)
        {
            ticket.alpha = visible ? 1f : 0f;
            ticket.blocksRaycasts = visible;
        }
    }
}
