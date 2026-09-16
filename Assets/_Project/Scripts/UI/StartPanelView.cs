using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Core;
using YesChef.Scoring;

namespace YesChef.UI
{
    public sealed class StartPanelView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TMP_Text highScoreText;

        private void OnEnable()
        {
            startButton.onClick.AddListener(gameManager.StartGame);
            quitButton.onClick.AddListener(gameManager.QuitGame);
            highScoreText.text = scoreManager.HighScore.ToString();
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(gameManager.StartGame);
            quitButton.onClick.RemoveListener(gameManager.QuitGame);
        }
    }
}
