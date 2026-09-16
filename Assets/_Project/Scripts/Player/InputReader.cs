using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YesChef.Player
{
    /// <summary>
    /// Adapts Input System actions into gameplay intents so gameplay code never references devices or bindings.
    /// </summary>
    public sealed class InputReader : MonoBehaviour
    {
        private const string GameplayMapName = "Gameplay";

        [SerializeField] private InputActionAsset inputActions;

        private InputActionMap gameplayMap;
        private InputAction moveAction;
        private InputAction interactAction;
        private InputAction pauseAction;

        public event Action InteractPressed;
        public event Action PausePressed;

        public Vector2 Move => moveAction.ReadValue<Vector2>();

        private void Awake()
        {
            gameplayMap = inputActions.FindActionMap(GameplayMapName, throwIfNotFound: true);
            moveAction = gameplayMap.FindAction("Move", throwIfNotFound: true);
            interactAction = gameplayMap.FindAction("Interact", throwIfNotFound: true);
            pauseAction = gameplayMap.FindAction("Pause", throwIfNotFound: true);
        }

        private void OnEnable()
        {
            interactAction.performed += OnInteractPerformed;
            pauseAction.performed += OnPausePerformed;
            gameplayMap.Enable();
        }

        private void OnDisable()
        {
            interactAction.performed -= OnInteractPerformed;
            pauseAction.performed -= OnPausePerformed;
            gameplayMap.Disable();
        }

        private void OnInteractPerformed(InputAction.CallbackContext _) => InteractPressed?.Invoke();

        private void OnPausePerformed(InputAction.CallbackContext _) => PausePressed?.Invoke();
    }
}
