using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YesChef.Core;
using YesChef.Scoring;

namespace YesChef.UI
{
    public sealed class GameOverView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private GameObject newHighScoreBadge;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;
        [SerializeField, Min(0f)] private float countUpSeconds = 1f;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(gameManager.StartGame);
            quitButton.onClick.AddListener(gameManager.QuitGame);

            // GameManager finalizes the round before switching state, so the result is ready when this panel opens.
            RoundResult result = scoreManager.LastRoundResult;
            highScoreText.text = result.HighScore.ToString();
            newHighScoreBadge.SetActive(false);
            StartCoroutine(Reveal(result));
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(gameManager.StartGame);
            quitButton.onClick.RemoveListener(gameManager.QuitGame);
        }

        private IEnumerator Reveal(RoundResult result)
        {
            for (float elapsed = 0f; elapsed < countUpSeconds; elapsed += Time.unscaledDeltaTime)
            {
                float t = 1f - Mathf.Pow(1f - elapsed / countUpSeconds, 3f);
                finalScoreText.text = Mathf.RoundToInt(result.Score * t).ToString();
                yield return null;
            }

            finalScoreText.text = result.Score.ToString();
            newHighScoreBadge.SetActive(result.IsNewHighScore);

            // Focus the restart button only after the reveal, so mashing Interact as the round ends can't restart it.
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
        }
    }
}
