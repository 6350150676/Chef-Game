using UnityEngine;
using UnityEngine.UI;
using YesChef.Core;

namespace YesChef.UI
{
    public sealed class PausePanelView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;

        private void OnEnable()
        {
            resumeButton.onClick.AddListener(gameManager.ResumeGame);
            quitButton.onClick.AddListener(gameManager.QuitGame);
        }

        private void OnDisable()
        {
            resumeButton.onClick.RemoveListener(gameManager.ResumeGame);
            quitButton.onClick.RemoveListener(gameManager.QuitGame);
        }
    }
}
