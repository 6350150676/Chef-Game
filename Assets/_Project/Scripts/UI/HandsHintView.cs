using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Ingredients;
using YesChef.Orders;
using YesChef.Player;
using YesChef.Stations;

namespace YesChef.UI
{
    /// <summary>Coaching bar: what the chef is holding and the next step for it.</summary>
    public sealed class HandsHintView : MonoBehaviour
    {
        [SerializeField] private PlayerHands hands;
        [SerializeField] private OrderManager orderManager;
        [Tooltip("Used to name the station an ingredient needs.")]
        [SerializeField] private ProcessingStation[] processingStations;

        [Header("Elements")]
        [SerializeField] private Image heldSwatch;
        [SerializeField] private TMP_Text heldText;
        [SerializeField] private TMP_Text nextStepText;
        [SerializeField] private UIPunchScale punch;
        [SerializeField] private Color emptySwatchColor = new Color(0.25f, 0.27f, 0.33f);
        [SerializeField, Min(0.05f)] private float refreshIntervalSeconds = 0.25f;

        private Ingredient shownIngredient;
        private bool shownPrepared;
        private bool shownWanted;
        private bool hasShown;
        private float nextRefreshTime;

        private void OnEnable()
        {
            hands.HeldIngredientChanged += HandleHeldIngredientChanged;
            hasShown = false;
            Refresh();
        }

        private void OnDisable() => hands.HeldIngredientChanged -= HandleHeldIngredientChanged;

        private void Update()
        {
            // Whether an order still wants the held food can change as orders complete or arrive.
            if (Time.unscaledTime >= nextRefreshTime)
                Refresh();
        }

        private void HandleHeldIngredientChanged(Ingredient ingredient)
        {
            Refresh();
            punch.Punch();
        }

        private void Refresh()
        {
            nextRefreshTime = Time.unscaledTime + refreshIntervalSeconds;

            Ingredient held = hands.HeldIngredient;
            bool isPrepared = held != null && held.IsPrepared;
            bool isWanted = held != null && orderManager.IsWantedByAnyOrder(held.Definition);

            // Only rebuild strings when what we'd say actually changes.
            if (hasShown && held == shownIngredient && isPrepared == shownPrepared && isWanted == shownWanted)
                return;

            hasShown = true;
            shownIngredient = held;
            shownPrepared = isPrepared;
            shownWanted = isWanted;

            if (held == null)
            {
                heldSwatch.color = emptySwatchColor;
                heldText.text = "Hands empty";
                nextStepText.text = "Grab an ingredient an order needs from the fridge";
                return;
            }

            heldSwatch.color = held.Definition.UIColor;
            heldText.text = $"Holding {held.DisplayName}";
            nextStepText.text = DescribeNextStep(held, isWanted);
        }

        private string DescribeNextStep(Ingredient held, bool isWanted)
        {
            if (!held.IsReadyToServe)
            {
                PreparationMethod method = held.Definition.RequiredPreparation;
                return $"{method.Verb()} it at the {StationNameFor(method)}";
            }

            return isWanted
                ? $"Serve it at a window that ordered {held.Definition.DisplayName}"
                : "No order needs this right now. Use the trash to free your hands";
        }

        private string StationNameFor(PreparationMethod method)
        {
            foreach (ProcessingStation station in processingStations)
            {
                if (station.Method == method)
                    return station.DisplayName;
            }

            return "right station";
        }
    }
}
