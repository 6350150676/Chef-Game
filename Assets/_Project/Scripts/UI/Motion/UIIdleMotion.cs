using UnityEngine;

namespace YesChef.UI
{
    /// <summary>Continuous decorative motion for menu art: spin, a breathing scale and a gentle vertical bob.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class UIIdleMotion : MonoBehaviour
    {
        [SerializeField] private float spinDegreesPerSecond;
        [SerializeField, Range(0f, 0.5f)] private float pulseAmount;
        [SerializeField] private float bobDistance;
        [SerializeField, Min(0f)] private float cyclesPerSecond = 0.5f;

        private RectTransform rectTransform;
        private Vector2 restPosition;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            restPosition = rectTransform.anchoredPosition;
        }

        private void Update()
        {
            float wave = Mathf.Sin(Time.unscaledTime * cyclesPerSecond * 2f * Mathf.PI);

            if (spinDegreesPerSecond != 0f)
                rectTransform.Rotate(0f, 0f, -spinDegreesPerSecond * Time.unscaledDeltaTime);

            if (pulseAmount > 0f)
                rectTransform.localScale = Vector3.one * (1f + pulseAmount * wave);

            if (bobDistance != 0f)
                rectTransform.anchoredPosition = restPosition + Vector2.up * (bobDistance * wave);
        }
    }
}
