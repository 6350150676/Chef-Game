using System;
using UnityEngine;
using YesChef.Core;
using YesChef.Orders;

namespace YesChef.Scoring
{
    /// <summary>
    /// Tracks the current score from completed orders and records the high score when a round ends.
    /// </summary>
    public sealed class ScoreManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private OrderManager orderManager;

        private IHighScoreRepository highScoreRepository;

        public int Score { get; private set; }
        public int HighScore { get; private set; }
        public RoundResult LastRoundResult { get; private set; }

        public event Action<int> ScoreChanged;
        public event Action<int> HighScoreChanged;

        private void Awake()
        {
            highScoreRepository = new PlayerPrefsHighScoreRepository();
            HighScore = highScoreRepository.Load();
        }

        private void OnEnable()
        {
            gameManager.RoundStarted += HandleRoundStarted;
            gameManager.RoundEnded += HandleRoundEnded;
            orderManager.OrderCompleted += HandleOrderCompleted;
        }

        private void OnDisable()
        {
            gameManager.RoundStarted -= HandleRoundStarted;
            gameManager.RoundEnded -= HandleRoundEnded;
            orderManager.OrderCompleted -= HandleOrderCompleted;
        }

        private void HandleRoundStarted() => SetScore(0);

        private void HandleOrderCompleted(OrderCompletion completion) => SetScore(Score + completion.Points);

        private void HandleRoundEnded()
        {
            bool isNewHighScore = Score > HighScore;
            if (isNewHighScore)
            {
                HighScore = Score;
                highScoreRepository.Save(HighScore);
                HighScoreChanged?.Invoke(HighScore);
            }

            LastRoundResult = new RoundResult(Score, HighScore, isNewHighScore);
        }

        private void SetScore(int score)
        {
            Score = score;
            ScoreChanged?.Invoke(Score);
        }
    }
}
