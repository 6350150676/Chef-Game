using UnityEngine;

namespace YesChef.Core
{
    /// <summary>
    /// Fits a fixed, tilted orthographic camera around the kitchen so the whole play area stays visible
    /// at any aspect ratio, with no camera movement during play.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class CameraFramer : MonoBehaviour
    {
        [Tooltip("Centre of the area to keep in frame, on the floor plane.")]
        [SerializeField] private Vector3 focusCenter;

        [Tooltip("Width (X) and depth (Z) of the area to keep in frame.")]
        [SerializeField] private Vector2 focusSize = new Vector2(20f, 12f);

        [SerializeField, Min(1f)] private float distance = 40f;

        private Camera cachedCamera;
        private float framedAspect;

        private void Awake()
        {
            cachedCamera = GetComponent<Camera>();
            cachedCamera.orthographic = true;
            Frame();
        }

        private void LateUpdate()
        {
            if (!Mathf.Approximately(cachedCamera.aspect, framedAspect))
                Frame();
        }

        private void Frame()
        {
            framedAspect = cachedCamera.aspect;

            // A floor depth D seen from a camera pitched down by angle p spans D * sin(p) of view height.
            float pitch = transform.eulerAngles.x * Mathf.Deg2Rad;
            float halfHeightForDepth = focusSize.y * Mathf.Sin(pitch) * 0.5f;
            float halfHeightForWidth = focusSize.x * 0.5f / framedAspect;

            cachedCamera.orthographicSize = Mathf.Max(halfHeightForDepth, halfHeightForWidth);
            transform.position = focusCenter - transform.forward * distance;
        }
    }
}
