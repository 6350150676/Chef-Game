using UnityEngine;
using YesChef.Stations;

namespace YesChef.Presentation
{
    /// <summary>World feedback for one station: focus highlight, guide marker, and a punch or shake when it's used.</summary>
    public sealed class StationFeedbackView : MonoBehaviour
    {
        [SerializeField] private InteractableStation station;
        [SerializeField] private GameObject focusHighlight;
        [SerializeField] private GameObject guideMarker;
        [SerializeField] private TransformTweener visualTweener;

        public InteractableStation Station => station;

        private void Awake()
        {
            focusHighlight.SetActive(false);
            guideMarker.SetActive(false);
        }

        private void OnEnable()
        {
            station.FocusChanged += HandleFocusChanged;
            station.Interacted += HandleInteracted;
        }

        private void OnDisable()
        {
            station.FocusChanged -= HandleFocusChanged;
            station.Interacted -= HandleInteracted;
        }

        public void SetGuideVisible(bool isVisible)
        {
            if (guideMarker.activeSelf != isVisible)
                guideMarker.SetActive(isVisible);
        }

        private void HandleFocusChanged(bool isFocused) => focusHighlight.SetActive(isFocused);

        private void HandleInteracted(InteractionPrompt prompt)
        {
            if (prompt.IsAvailable)
                visualTweener.Punch(0.08f);
            else
                visualTweener.Shake(0.06f);
        }
    }
}
