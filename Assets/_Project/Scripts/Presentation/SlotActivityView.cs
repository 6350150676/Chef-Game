using UnityEngine;
using YesChef.Ingredients;
using YesChef.Stations;

namespace YesChef.Presentation
{
    /// <summary>Shows a slot working (glowing burner, moving knife) and pops the food when it's ready.</summary>
    public sealed class SlotActivityView : MonoBehaviour
    {
        [SerializeField] private ProcessingStation station;
        [SerializeField, Min(0)] private int slotIndex;

        [Tooltip("Visible only while this slot is actively preparing.")]
        [SerializeField] private GameObject workingIndicator;

        private void Awake() => workingIndicator.SetActive(false);

        private void OnEnable() => station.SlotCompleted += HandleSlotCompleted;

        private void OnDisable() => station.SlotCompleted -= HandleSlotCompleted;

        private void LateUpdate()
        {
            ProcessingSlot slot = station.GetSlot(slotIndex);
            bool isWorking = slot.IsProcessing && !station.IsStalled;
            if (workingIndicator.activeSelf != isWorking)
                workingIndicator.SetActive(isWorking);
        }

        private void HandleSlotCompleted(ProcessingStation completedStation, int completedSlot)
        {
            if (completedSlot != slotIndex)
                return;

            Ingredient ingredient = station.GetSlot(slotIndex).CurrentIngredient;
            if (ingredient != null && ingredient.TryGetComponent(out TransformTweener tweener))
                tweener.Punch(0.4f);
        }
    }
}
