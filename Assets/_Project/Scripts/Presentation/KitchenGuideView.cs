using UnityEngine;
using YesChef.Core;
using YesChef.Orders;
using YesChef.Player;
using YesChef.Stations;

namespace YesChef.Presentation
{
    /// <summary>
    /// Floats markers over the stations that move the chef's current task forward: where the held ingredient can go,
    /// finished food waiting to be collected, or the fridge compartments that orders still need.
    /// </summary>
    public sealed class KitchenGuideView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private PlayerHands hands;
        [SerializeField] private StationFeedbackView[] stations;
        [SerializeField, Min(0.02f)] private float refreshIntervalSeconds = 0.15f;

        private float nextRefreshTime;

        private void Update()
        {
            if (Time.unscaledTime < nextRefreshTime)
                return;

            nextRefreshTime = Time.unscaledTime + refreshIntervalSeconds;
            Refresh();
        }

        private void Refresh()
        {
            if (!gameManager.IsPlaying)
            {
                ShowWhere(_ => false);
                return;
            }

            if (hands.IsHoldingIngredient)
            {
                bool hasDestination = ShowWhere(kind => kind == InteractionKind.StartPreparing || kind == InteractionKind.Serve);

                // Finished food that no order wants belongs in the trash.
                if (!hasDestination && hands.HeldIngredient.IsReadyToServe)
                    ShowWhere(kind => kind == InteractionKind.Discard);
            }
            else if (!ShowWhere(kind => kind == InteractionKind.CollectPrepared))
            {
                ShowWantedFridgeCompartments();
            }
        }

        /// <returns>True if at least one marker is shown.</returns>
        private bool ShowWhere(System.Func<InteractionKind, bool> isUseful)
        {
            bool anyShown = false;
            foreach (StationFeedbackView view in stations)
            {
                InteractableStation station = view.Station;
                bool show = !station.IsFocused && isUseful(station.GetPrompt(hands).Kind);
                view.SetGuideVisible(show);
                anyShown |= show;
            }

            return anyShown;
        }

        private void ShowWantedFridgeCompartments()
        {
            foreach (StationFeedbackView view in stations)
            {
                bool show = view.Station is RefrigeratorCompartment compartment
                            && !compartment.IsFocused
                            && orderManager.IsWantedByAnyOrder(compartment.Ingredient);
                view.SetGuideVisible(show);
            }
        }
    }
}
