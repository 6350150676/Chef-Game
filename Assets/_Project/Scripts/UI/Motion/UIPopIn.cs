using UnityEngine;

namespace YesChef.UI
{
    /// <summary>
    /// Pops an element in when it appears: scales up with a slight overshoot and optionally fades in.
    /// Runs on unscaled time so panels still animate while the game is paused.
    /// </summary>
    public sealed class UIPopIn : MonoBehaviour
    {
        [Tooltip("Optional. Faded in alongside the scale.")]
        [SerializeField] private CanvasGroup fade;
        [SerializeField, Min(0.01f)] private float durationSeconds = 0.35f;
        [SerializeField, Min(0f)] private float delaySeconds;
        [SerializeField] private float startScale = 0.75f;
        [SerializeField] private bool playOnEnable = true;

        private float elapsed;
        private bool isPlaying;

        private void OnEnable()
        {
            if (playOnEnable)
                Play();
        }

        private void OnDisable()
        {
            isPlaying = false;
            Apply(1f);
        }

        public void Play()
        {
            elapsed = -delaySeconds;
            isPlaying = true;
            Apply(0f);
        }

        private void Update()
        {
            if (!isPlaying)
                return;

            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / durationSeconds);
            Apply(t);
            isPlaying = t < 1f;
        }

        private void Apply(float t)
        {
            transform.localScale = Vector3.one * Mathf.LerpUnclamped(startScale, 1f, EaseOutBack(t));
            if (fade != null)
                fade.alpha = Mathf.Clamp01(t * 2f);
        }

        private static float EaseOutBack(float t)
        {
            const float overshoot = 1.70158f;
            float u = t - 1f;
            return 1f + (overshoot + 1f) * u * u * u + overshoot * u * u;
        }
    }
}
