using UnityEngine;
using YesChef.Ingredients;
using YesChef.Stations;

namespace YesChef.UI
{
    /// <summary>Calls out kitchen events that happen away from the chef, like "Cooked!" when a burner finishes.</summary>
    public sealed class KitchenCalloutsView : MonoBehaviour
    {
        [SerializeField] private FloatingTextSpawner floatingText;
        [SerializeField] private ProcessingStation[] stations;
        [SerializeField] private Color readyColor = new Color(0.55f, 1f, 0.5f);
        [SerializeField] private Vector3 calloutOffset = new Vector3(0f, 1.1f, 0f);

        private void OnEnable()
        {
            foreach (ProcessingStation station in stations)
                station.SlotCompleted += HandleSlotCompleted;
        }

        private void OnDisable()
        {
            foreach (ProcessingStation station in stations)
                station.SlotCompleted -= HandleSlotCompleted;
        }

        private void HandleSlotCompleted(ProcessingStation station, int slotIndex)
        {
            Vector3 position = station.GetSlot(slotIndex).Position + calloutOffset;
            floatingText.Spawn(position, station.Method.DoneCallout(), readyColor);
        }
    }
}
