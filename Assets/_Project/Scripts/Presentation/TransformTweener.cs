using UnityEngine;

namespace YesChef.Presentation
{
    /// <summary>
    /// Small procedural feedback animations (punch, shake, pop-in) that work on world objects and UI alike.
    /// Runs on unscaled time so feedback still plays while the game is paused.
    /// </summary>
    public sealed class TransformTweener : MonoBehaviour
    {
        private enum Effect
        {
            None,
            Punch,
            Shake,
            PopIn
        }

        [SerializeField, Min(0.01f)] private float durationSeconds = 0.3f;

        private Vector3 baseScale;
        private Vector3 basePosition;
        private bool hasBase;
        private Effect effect;
        private float strength;
        private float elapsed;

        private void Awake() => CaptureBase();

        /// <summary>Briefly scales up and settles. Strength is the relative scale overshoot (0.2 = 20%).</summary>
        public void Punch(float scaleStrength) => Play(Effect.Punch, scaleStrength);

        /// <summary>Wiggles sideways. Strength is in local units (pixels for UI, meters for world objects).</summary>
        public void Shake(float distance) => Play(Effect.Shake, distance);

        /// <summary>Grows from nothing with a slight overshoot.</summary>
        public void PopIn() => Play(Effect.PopIn, 1f);

        private void Play(Effect next, float amount)
        {
            CaptureBase();
            RestoreBase();
            effect = next;
            strength = amount;
            elapsed = 0f;
            enabled = true;
            Apply(0f);
        }

        private void Update()
        {
            if (effect == Effect.None)
            {
                enabled = false;
                return;
            }

            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / durationSeconds);
            Apply(t);

            if (t >= 1f)
            {
                RestoreBase();
                effect = Effect.None;
                enabled = false;
            }
        }

        private void OnDisable()
        {
            if (effect != Effect.None)
            {
                RestoreBase();
                effect = Effect.None;
            }
        }

        private void Apply(float t)
        {
            float falloff = 1f - t;
            switch (effect)
            {
                case Effect.Punch:
                    transform.localScale = baseScale * (1f + Mathf.Sin(t * Mathf.PI * 2f) * strength * falloff);
                    break;
                case Effect.Shake:
                    transform.localPosition = basePosition + Vector3.right * (Mathf.Sin(t * Mathf.PI * 8f) * strength * falloff);
                    break;
                case Effect.PopIn:
                    transform.localScale = baseScale * EaseOutBack(t);
                    break;
            }
        }

        private void CaptureBase()
        {
            if (hasBase)
                return;

            baseScale = transform.localScale;
            basePosition = transform.localPosition;
            hasBase = true;
        }

        private void RestoreBase()
        {
            transform.localScale = baseScale;
            transform.localPosition = basePosition;
        }

        private static float EaseOutBack(float t)
        {
            const float overshoot = 1.70158f;
            float x = t - 1f;
            return 1f + (overshoot + 1f) * x * x * x + overshoot * x * x;
        }
    }
}
