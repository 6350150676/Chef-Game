using System;
using TMPro;
using UnityEngine;

namespace YesChef.UI
{
    /// <summary>A short callout that pops up over a world position, rises and fades. Managed by <see cref="FloatingTextSpawner"/>.</summary>
    public sealed class FloatingTextUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField, Min(0.1f)] private float durationSeconds = 1.2f;
        [SerializeField] private float riseDistance = 70f;

        private RectTransform rectTransform;
        private Canvas rootCanvas;
        private Camera worldCamera;
        private Action<FloatingTextUI> onFinished;
        private Vector3 worldPosition;
        private float elapsed;

        public void Initialize(Canvas canvas, Camera camera, Action<FloatingTextUI> finished)
        {
            rectTransform = (RectTransform)transform;
            rootCanvas = canvas;
            worldCamera = camera;
            onFinished = finished;
        }

        public void Play(Vector3 position, string text, Color color)
        {
            worldPosition = position;
            label.text = text;
            label.color = color;
            elapsed = 0f;
            Apply(0f);
        }

        private void Update()
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / durationSeconds;
            if (t >= 1f)
            {
                onFinished(this);
                return;
            }

            Apply(t);
        }

        private void Apply(float t)
        {
            Vector2 rise = Vector2.up * (riseDistance * rootCanvas.scaleFactor * Mathf.Sqrt(t));
            UIPlacement.PlaceAtWorldPoint(rectTransform, rootCanvas, worldCamera, worldPosition, rise);

            float scale = t < 0.15f ? Mathf.Lerp(0.5f, 1.2f, t / 0.15f) : Mathf.Lerp(1.2f, 1f, (t - 0.15f) / 0.2f);
            rectTransform.localScale = new Vector3(scale, scale, 1f);
            label.alpha = t < 0.65f ? 1f : 1f - (t - 0.65f) / 0.35f;
        }
    }
}
