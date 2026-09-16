using System;
using UnityEngine;

namespace YesChef.Core
{
    /// <summary>
    /// Owns the game state machine and the round clock. Other systems observe its events rather than
    /// being called from here, so new systems can join the round lifecycle without modifying this class.
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private GameConfig config;

        private readonly CountdownTimer roundTimer = new CountdownTimer();

        public GameState State { get; private set; } = GameState.MainMenu;
        public bool IsPlaying => State == GameState.Playing;
        public float TimeRemaining => roundTimer.Remaining;
        public float RoundDuration => roundTimer.Duration;

        public event Action<GameState> StateChanged;

        /// <summary>Raised before the state switches to Playing, so systems reset before views appear.</summary>
        public event Action RoundStarted;

        /// <summary>Raised before the state switches to GameOver, so results are final before views appear.</summary>
        public event Action RoundEnded;

        private void Awake() => Time.timeScale = 1f;

        private void OnDestroy() => Time.timeScale = 1f;

        private void Update()
        {
            if (IsPlaying && roundTimer.Tick(Time.deltaTime))
                EndRound();
        }

        /// <summary>Starts a fresh round from the main menu or the game over screen.</summary>
        public void StartGame()
        {
            if (State == GameState.Playing || State == GameState.Paused)
                return;

            Time.timeScale = 1f;
            roundTimer.Start(config.RoundDurationSeconds);
            RoundStarted?.Invoke();
            ChangeState(GameState.Playing);
        }

        public void PauseGame()
        {
            if (State != GameState.Playing)
                return;

            Time.timeScale = 0f;
            ChangeState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (State != GameState.Paused)
                return;

            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
        }

        public void TogglePause()
        {
            if (State == GameState.Playing)
                PauseGame();
            else
                ResumeGame();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void EndRound()
        {
            RoundEnded?.Invoke();
            ChangeState(GameState.GameOver);
        }

        private void ChangeState(GameState next)
        {
            if (State == next)
                return;

            State = next;
            StateChanged?.Invoke(next);
        }
    }
}
