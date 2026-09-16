using UnityEngine;

namespace YesChef.Presentation
{
    /// <summary>Idle bob and spin for markers and animated props (e.g. a chopping knife).</summary>
    public sealed class Bobber : MonoBehaviour
    {
        [SerializeField] private float bobHeight = 0.12f;
        [SerializeField] private float bobsPerSecond = 1.5f;
        [SerializeField] private float spinDegreesPerSecond = 120f;

        private Vector3 basePosition;

        private void Awake() => basePosition = transform.localPosition;

        private void Update()
        {
            float wave = Mathf.Sin(Time.time * bobsPerSecond * Mathf.PI * 2f);
            transform.localPosition = basePosition + Vector3.up * (wave * bobHeight);

            if (spinDegreesPerSecond != 0f)
                transform.Rotate(0f, spinDegreesPerSecond * Time.deltaTime, 0f, Space.Self);
        }
    }
}
