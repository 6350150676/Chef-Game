namespace YesChef.Scoring
{
    /// <summary>Persistence boundary for the high score, so storage (PlayerPrefs, file, cloud) can change freely.</summary>
    public interface IHighScoreRepository
    {
        int Load();
        void Save(int highScore);
    }
}
