using TMPro;
using UnityEngine;
using YesChef.Player;
using YesChef.Stations;

namespace YesChef.UI
{
    /// <summary>
    /// Shows what pressing Interact will do at the station in front of the chef ("[E] Chop Veggie"),
    /// or why nothing will happen ("Needs cooking first").
    /// </summary>
    public sealed class InteractionPromptView : MonoBehaviour
    {
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private PlayerHands hands;

        [Header("Elements")]
        [SerializeField] private UIWorldAnchor anchor;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject keyCap;
        [SerializeField] private TMP_Text label;
        [SerializeField] private UIPopIn appearMotion;
        [SerializeField] private UIPunchScale rejectPunch;

        [Header("Style")]
        [SerializeField] private Color availableTextColor = Color.white;
        [SerializeField] private Color blockedTextColor = new Color(1f, 0.72f, 0.65f);
        [SerializeField, Min(0.02f)] private float refreshIntervalSeconds = 0.1f;

        private float nextRefreshTime;

        private void OnEnable()
        {
            interactor.TargetChanged += HandleTargetChanged;
            interactor.Interacted += HandleInteracted;
            Refresh();
        }

        private void OnDisable()
        {
            interactor.TargetChanged -= HandleTargetChanged;
            interactor.Interacted -= HandleInteracted;
        }

        private void Update()
        {
            // Station state changes over time (food finishing, orders arriving), so re-read the prompt periodically.
            if (Time.unscaledTime >= nextRefreshTime)
                Refresh();
        }

        private void HandleTargetChanged(IInteractable target)
        {
            Refresh();
            if (target != null)
                appearMotion.Play();
        }

        private void HandleInteracted(IInteractable target, bool succeeded)
        {
            Refresh();
            if (target != null && !succeeded)
                rejectPunch.Punch();
        }

        private void Refresh()
        {
            nextRefreshTime = Time.unscaledTime + refreshIntervalSeconds;

            IInteractable target = interactor.CurrentTarget;
            canvasGroup.alpha = target != null ? 1f : 0f;
            if (target == null)
                return;

            InteractionPrompt prompt = target.GetPrompt(hands);
            anchor.SetWorldPoint(target.PromptPosition);

            if (keyCap.activeSelf != prompt.IsAvailable)
                keyCap.SetActive(prompt.IsAvailable);

            label.text = prompt.Text;
            label.color = prompt.IsAvailable ? availableTextColor : blockedTextColor;
        }
    }
}
