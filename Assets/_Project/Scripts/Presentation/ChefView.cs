using UnityEngine;
using YesChef.Ingredients;
using YesChef.Player;
using YesChef.Stations;

namespace YesChef.Presentation
{
    /// <summary>Procedural chef animation: bob and lean while walking, react to interactions and pickups.</summary>
    public sealed class ChefView : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private PlayerHands hands;
        [SerializeField] private Transform bobRoot;
        [SerializeField] private TransformTweener modelTweener;

        [Header("Walk")]
        [SerializeField] private float bobHeight = 0.09f;
        [SerializeField] private float stepsPerSecond = 3.5f;
        [SerializeField] private float leanDegrees = 10f;

        private Vector3 bobRootBase;
        private float walkCycle;
        private float smoothedSpeed;

        private void Awake() => bobRootBase = bobRoot.localPosition;

        private void OnEnable()
        {
            interactor.Interacted += HandleInteracted;
            hands.HeldIngredientChanged += HandleHeldIngredientChanged;
        }

        private void OnDisable()
        {
            interactor.Interacted -= HandleInteracted;
            hands.HeldIngredientChanged -= HandleHeldIngredientChanged;
        }

        private void Update()
        {
            smoothedSpeed = Mathf.MoveTowards(smoothedSpeed, movement.Speed01, Time.deltaTime * 8f);
            walkCycle += Time.deltaTime * stepsPerSecond * Mathf.PI * smoothedSpeed;

            float bob = Mathf.Abs(Mathf.Sin(walkCycle)) * bobHeight * smoothedSpeed;
            bobRoot.localPosition = bobRootBase + Vector3.up * bob;
            bobRoot.localRotation = Quaternion.Euler(leanDegrees * smoothedSpeed, 0f, 0f);
        }

        private void HandleInteracted(IInteractable target, bool succeeded)
        {
            if (succeeded)
                modelTweener.Punch(0.12f);
            else
                modelTweener.Shake(0.05f);
        }

        private static void HandleHeldIngredientChanged(Ingredient ingredient)
        {
            if (ingredient != null && ingredient.TryGetComponent(out TransformTweener tweener))
                tweener.Punch(0.35f);
        }
    }
}
