using UnityEngine;
using YesChef.Core;

namespace YesChef.Player
{
    /// <summary>
    /// Composes the chef's parts: feeds input to movement and interaction while a round is being played,
    /// and returns the chef to the start when a new round begins. The parts themselves know nothing about game state.
    /// </summary>
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private InputReader input;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private PlayerHands hands;

        private Vector3 spawnPosition;
        private Quaternion spawnRotation;

        private void Awake()
        {
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
        }

        private void OnEnable()
        {
            gameManager.RoundStarted += HandleRoundStarted;
            gameManager.RoundEnded += HandleRoundEnded;
            input.InteractPressed += HandleInteractPressed;
        }

        private void OnDisable()
        {
            gameManager.RoundStarted -= HandleRoundStarted;
            gameManager.RoundEnded -= HandleRoundEnded;
            input.InteractPressed -= HandleInteractPressed;
        }

        private void Update()
        {
            if (!gameManager.IsPlaying)
            {
                movement.Stop();
                return;
            }

            movement.Move(input.Move, Time.deltaTime);
            interactor.RefreshTarget();
        }

        private void HandleInteractPressed()
        {
            if (gameManager.IsPlaying)
                interactor.Interact(hands);
        }

        private void HandleRoundStarted()
        {
            hands.DiscardHeldIngredient();
            interactor.ClearTarget();
            movement.Teleport(spawnPosition, spawnRotation);
        }

        private void HandleRoundEnded() => interactor.ClearTarget();
    }
}
