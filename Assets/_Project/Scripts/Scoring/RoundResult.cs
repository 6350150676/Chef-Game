namespace YesChef.Scoring
{
    public readonly struct RoundResult
    {
        public RoundResult(int score, int highScore, bool isNewHighScore)
        {
            Score = score;
            HighScore = highScore;
            IsNewHighScore = isNewHighScore;
        }

        public int Score { get; }
        public int HighScore { get; }
        public bool IsNewHighScore { get; }
    }
}
