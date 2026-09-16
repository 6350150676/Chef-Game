using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Stations;

namespace YesChef.UI
{
    /// <summary>World-space progress bar above one station slot, showing time remaining until the ingredient is ready.</summary>
    public sealed class SlotProgressView : MonoBehaviour
    {
        [SerializeField] private ProcessingStation station;
        [SerializeField, Min(0)] private int slotIndex;

        [Header("Elements")]
        [SerializeField] private GameObject bar;
        [SerializeField] private RectTransform fill;
        [SerializeField] private Image fillImage;
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject readyBadge;

        [Header("Colors")]
        [SerializeField] private Color workingColor = new Color(1f, 0.75f, 0.2f);
        [SerializeField] private Color stalledColor = new Color(0.6f, 0.6f, 0.6f);
        [SerializeField] private Color readyColor = new Color(0.35f, 0.9f, 0.4f);

        private void LateUpdate()
        {
            ProcessingSlot slot = station.GetSlot(slotIndex);

            bool isVisible = !slot.IsEmpty;
            SetActive(bar, isVisible);
            SetActive(readyBadge, slot.IsDone);

            if (!isVisible)
                return;

            fill.anchorMax = new Vector2(slot.Progress01, 1f);

            if (slot.IsDone)
            {
                fillImage.color = readyColor;
                label.text = "Ready!";
            }
            else
            {
                fillImage.color = station.IsStalled ? stalledColor : workingColor;
                label.SetText("{0:1}s", slot.RemainingSeconds);
            }
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target.activeSelf != active)
                target.SetActive(active);
        }
    }
}
