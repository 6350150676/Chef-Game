using UnityEngine;

namespace YesChef.UI
{
    internal static class UIPlacement
    {
        /// <summary>
        /// Moves a UI element over a world position. Works for both Screen Space Overlay and Screen Space Camera canvases.
        /// </summary>
        public static void PlaceAtWorldPoint(RectTransform element, Canvas rootCanvas, Camera worldCamera, Vector3 worldPoint,
            Vector2 screenOffset = default)
        {
            Vector2 screenPoint = (Vector2)worldCamera.WorldToScreenPoint(worldPoint) + screenOffset;
            Camera uiCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
            var parent = (RectTransform)element.parent;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, screenPoint, uiCamera, out Vector3 position))
                element.position = position;
        }
    }
}
