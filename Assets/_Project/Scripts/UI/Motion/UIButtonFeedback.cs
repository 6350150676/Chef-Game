using UnityEngine;
using UnityEngine.EventSystems;

namespace YesChef.UI
{
    /// <summary>
    /// Makes a button feel physical: it grows while hovered or selected, squashes while held, and pushes its
    /// content down to match the pressed sprite. Unscaled time, so it responds the same in the pause menu.
    /// </summary>
    public sealed class UIButtonFeedback : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private float focusedScale = 1.06f;
        [SerializeField] private float pressedScale = 0.95f;
        [SerializeField, Min(1f)] private float responsiveness = 20f;

        [Tooltip("Optional. The label/icon group, moved down while pressed.")]
        [SerializeField] private RectTransform content;
        [SerializeField] private float pressedContentOffset = -5f;

        private bool isHovered;
        private bool isSelected;
        private bool isPressed;
        private Vector2 contentRestPosition;

        private void Awake()
        {
            if (content != null)
                contentRestPosition = content.anchoredPosition;
        }

        private void OnDisable()
        {
            isHovered = isSelected = isPressed = false;
            transform.localScale = Vector3.one;
            if (content != null)
                content.anchoredPosition = contentRestPosition;
        }

        private void Update()
        {
            float target = isPressed ? pressedScale : isHovered || isSelected ? focusedScale : 1f;
            float blend = 1f - Mathf.Exp(-responsiveness * Time.unscaledDeltaTime);
            transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, target, blend);
        }

        public void OnPointerEnter(PointerEventData eventData) => isHovered = true;

        public void OnPointerExit(PointerEventData eventData) => isHovered = false;

        public void OnPointerDown(PointerEventData eventData) => SetPressed(true);

        public void OnPointerUp(PointerEventData eventData) => SetPressed(false);

        public void OnSelect(BaseEventData eventData) => isSelected = true;

        public void OnDeselect(BaseEventData eventData) => isSelected = false;

        private void SetPressed(bool pressed)
        {
            isPressed = pressed;
            if (content != null)
                content.anchoredPosition = contentRestPosition + Vector2.up * (pressed ? pressedContentOffset : 0f);
        }
    }
}
