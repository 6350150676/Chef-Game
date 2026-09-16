using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Core;
using YesChef.Scoring;

namespace YesChef.UI
{
    public sealed class HUDView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ScoreManager scoreManager;

        [Header("Elements")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private TMP_Text gameTimerText;
        [SerializeField] private Image timerRing;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button quitButton;

        [Header("Feedback")]
        [SerializeField] private UIPunchScale scorePunch;
        [SerializeField] private UIPunchScale timerPunch;
        [Tooltip("Below this many seconds the clock turns red and ticks visibly.")]
        [SerializeField, Min(0)] private int lowTimeSeconds = 30;
        [SerializeField] private Color timerColor = Color.white;
        [SerializeField] private Color lowTimeColor = new Color(1f, 0.36f, 0.25f);
        [SerializeField] private Gradient ringColors = new Gradient();

        private int displayedSeconds = -1;

        private void OnEnable()
        {
            scoreManager.ScoreChanged += HandleScoreChanged;
            scoreManager.HighScoreChanged += ShowHighScore;
            pauseButton.onClick.AddListener(gameManager.PauseGame);
            quitButton.onClick.AddListener(gameManager.QuitGame);

            scoreText.text = scoreManager.Score.ToString();
            ShowHighScore(scoreManager.HighScore);
            displayedSeconds = -1;
        }

        private void OnDisable()
        {
            scoreManager.ScoreChanged -= HandleScoreChanged;
            scoreManager.HighScoreChanged -= ShowHighScore;
            pauseButton.onClick.RemoveListener(gameManager.PauseGame);
            quitButton.onClick.RemoveListener(gameManager.QuitGame);
        }

        private void Update()
        {
            float remaining01 = gameManager.RoundDuration > 0f ? gameManager.TimeRemaining / gameManager.RoundDuration : 0f;
            timerRing.fillAmount = remaining01;
            timerRing.color = ringColors.Evaluate(1f - remaining01);

            // Only rebuild the label when the visible value changes.
            int seconds = Mathf.CeilToInt(gameManager.TimeRemaining);
            if (seconds == displayedSeconds)
                return;

            bool isLow = seconds <= lowTimeSeconds;
            if (isLow && displayedSeconds > seconds && gameManager.IsPlaying)
                timerPunch.Punch();

            displayedSeconds = seconds;
            gameTimerText.text = $"{seconds / 60}:{seconds % 60:00}";
            gameTimerText.color = isLow ? lowTimeColor : timerColor;
        }

        private void HandleScoreChanged(int score)
        {
            scoreText.text = score.ToString();
            scorePunch.Punch();
        }

        private void ShowHighScore(int highScore) => highScoreText.text = highScore.ToString();
    }
}
