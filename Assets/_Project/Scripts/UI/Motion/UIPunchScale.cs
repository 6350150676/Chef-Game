using UnityEngine;

namespace YesChef.UI
{
    /// <summary>A quick scale bump that draws the eye to a value that just changed.</summary>
    public sealed class UIPunchScale : MonoBehaviour
    {
        [SerializeField] private float strength = 0.2f;
        [SerializeField, Min(0.01f)] private float durationSeconds = 0.25f;

        private float elapsed = float.MaxValue;

        public void Punch() => elapsed = 0f;

        private void OnDisable()
        {
            elapsed = float.MaxValue;
            transform.localScale = Vector3.one;
        }

        private void Update()
        {
            if (elapsed > durationSeconds)
                return;

            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / durationSeconds);
            transform.localScale = Vector3.one * (1f + strength * Mathf.Sin(t * Mathf.PI));
        }
    }
}
