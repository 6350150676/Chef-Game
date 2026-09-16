using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YesChef.Core;
using YesChef.Player;

namespace YesChef.UI
{
    /// <summary>Shows the panels that match the current game state and handles the pause shortcut.</summary>
    public sealed class UIManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private InputReader input;

        [Header("Panels")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject gameHud;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Gameplay coaching")]
        [Tooltip("Optional. Prompts and hints that only make sense while a round is in progress.")]
        [SerializeField] private GameObject gameplayHints;
        [Tooltip("Optional. Announces the start of the round and the final seconds.")]
        [SerializeField] private RoundBannerView roundBanner;

        [Header("Default focus for keyboard and gamepad")]
        [SerializeField] private Selectable startPanelFocus;
        [SerializeField] private Selectable pausePanelFocus;

        private GameState appliedState;

        private void OnEnable()
        {
            gameManager.StateChanged += ApplyState;
            input.PausePressed += gameManager.TogglePause;
        }

        private void OnDisable()
        {
            gameManager.StateChanged -= ApplyState;
            input.PausePressed -= gameManager.TogglePause;
        }

        private void Start()
        {
            appliedState = gameManager.State;
            ApplyState(gameManager.State);
        }

        private void ApplyState(GameState state)
        {
            GameState previous = appliedState;
            appliedState = state;

            startPanel.SetActive(state == GameState.MainMenu);
            // The HUD stays up behind the pause and game over panels so the kitchen state remains readable.
            gameHud.SetActive(state != GameState.MainMenu);
            pausePanel.SetActive(state == GameState.Paused);
            gameOverPanel.SetActive(state == GameState.GameOver);

            if (gameplayHints != null)
                gameplayHints.SetActive(state == GameState.Playing || state == GameState.Paused);

            if (roundBanner != null && state == GameState.Playing && previous != GameState.Paused)
                roundBanner.ShowRoundStart();

            if (EventSystem.current == null)
                return;

            // Menus focus their main button for gamepad navigation. Gameplay clears focus, otherwise a clicked button
            // keeps it and Space/Enter would press it again. The game over panel focuses itself after its reveal.
            Selectable focus = state switch
            {
                GameState.MainMenu => startPanelFocus,
                GameState.Paused => pausePanelFocus,
                _ => null,
            };
            EventSystem.current.SetSelectedGameObject(focus != null ? focus.gameObject : null);
        }
    }
}
