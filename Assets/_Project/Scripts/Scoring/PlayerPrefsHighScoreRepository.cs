using UnityEngine;

namespace YesChef.Scoring
{
    public sealed class PlayerPrefsHighScoreRepository : IHighScoreRepository
    {
        private const string HighScoreKey = "YesChef.HighScore";

        public int Load() => PlayerPrefs.GetInt(HighScoreKey, 0);

        public void Save(int highScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }
    }
}
