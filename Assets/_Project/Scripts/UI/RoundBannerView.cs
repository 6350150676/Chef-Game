using TMPro;
using UnityEngine;
using YesChef.Core;

namespace YesChef.UI
{
    /// <summary>Big centre-screen callouts: the round starting and the final seconds warning.</summary>
    public sealed class RoundBannerView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text label;
        [SerializeField] private UIPopIn popIn;

        [Header("Content")]
        [SerializeField] private string roundStartText = "Let's cook!";
        [SerializeField] private string finalSecondsText = "10 seconds left!";
        [SerializeField, Min(0f)] private float finalSecondsThreshold = 10f;
        [SerializeField, Min(0.1f)] private float displaySeconds = 1.4f;
        [SerializeField, Min(0.01f)] private float fadeSeconds = 0.35f;

        private float hideTime;
        private bool hasWarned;

        private void Awake() => canvasGroup.alpha = 0f;

        public void ShowRoundStart()
        {
            hasWarned = false;
            Show(roundStartText);
        }

        private void Update()
        {
            if (gameManager.IsPlaying && !hasWarned && gameManager.TimeRemaining <= finalSecondsThreshold)
            {
                hasWarned = true;
                Show(finalSecondsText);
            }

            if (canvasGroup.alpha > 0f)
                canvasGroup.alpha = Mathf.Clamp01((hideTime - Time.unscaledTime) / fadeSeconds);
        }

        private void Show(string text)
        {
            label.text = text;
            hideTime = Time.unscaledTime + displaySeconds;
            canvasGroup.alpha = 1f;
            popIn.Play();
        }
    }
}
