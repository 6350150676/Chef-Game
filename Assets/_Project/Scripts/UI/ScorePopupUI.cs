using TMPro;
using UnityEngine;

namespace YesChef.UI
{
    /// <summary>Floating "+17" / "-6" label that pops in, drifts upward and fades out.</summary>
    public sealed class ScorePopupUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private Color gainColor = new Color(0.4f, 1f, 0.45f);
        [SerializeField] private Color lossColor = new Color(1f, 0.35f, 0.3f);
        [SerializeField, Min(0.1f)] private float durationSeconds = 2f;
        [SerializeField] private float riseDistance = 60f;
        [SerializeField] private AnimationCurve scaleOverLifetime = new AnimationCurve(
            new Keyframe(0f, 0.3f), new Keyframe(0.1f, 1.3f), new Keyframe(0.2f, 1f), new Keyframe(1f, 1f));

        private RectTransform rectTransform;
        private Vector2 restPosition;
        private float elapsed;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            restPosition = rectTransform.anchoredPosition;
        }

        public void Play(int points)
        {
            label.text = points >= 0 ? $"+{points}" : points.ToString();
            label.color = points >= 0 ? gainColor : lossColor;
            elapsed = 0f;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / durationSeconds);

            rectTransform.anchoredPosition = restPosition + Vector2.up * (riseDistance * t);
            rectTransform.localScale = Vector3.one * scaleOverLifetime.Evaluate(t);
            // Hold full opacity for the first half, then fade out.
            label.alpha = t < 0.5f ? 1f : 1f - (t - 0.5f) * 2f;

            if (t >= 1f)
                gameObject.SetActive(false);
        }
    }
}
