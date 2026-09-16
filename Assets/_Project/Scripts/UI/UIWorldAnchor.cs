using UnityEngine;

namespace YesChef.UI
{
    /// <summary>Pins a screen-space UI element next to a world object, such as an order ticket beside its window.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class UIWorldAnchor : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 worldOffset;

        private RectTransform rectTransform;
        private RectTransform parentRect;
        private Canvas canvas;
        private Camera worldCamera;
        private Vector3 fixedWorldPoint;
        private bool useFixedWorldPoint;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            parentRect = (RectTransform)rectTransform.parent;
            canvas = GetComponentInParent<Canvas>().rootCanvas;
            worldCamera = Camera.main;
        }

        /// <summary>Pins the element to a fixed world position instead of following a transform.</summary>
        public void SetWorldPoint(Vector3 worldPoint)
        {
            fixedWorldPoint = worldPoint;
            useFixedWorldPoint = true;
            LateUpdate();
        }

        private void LateUpdate()
        {
            if (worldCamera == null || (!useFixedWorldPoint && target == null))
                return;

            Vector3 anchorPoint = useFixedWorldPoint ? fixedWorldPoint : target.position + worldOffset;
            Vector2 screenPoint = worldCamera.WorldToScreenPoint(anchorPoint);
            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, screenPoint, uiCamera, out Vector3 worldPoint))
                rectTransform.position = worldPoint;
        }
    }
}
