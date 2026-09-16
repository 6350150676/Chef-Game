using System;
using UnityEngine;
using YesChef.Ingredients;
using YesChef.Stations;

namespace YesChef.Player
{
    /// <summary>
    /// Targets the nearest interactable in front of the chef, keeps it focused and forwards interactions to it.
    /// </summary>
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [Tooltip("Point in front of the chef that interactables are measured from.")]
        [SerializeField] private Transform probeOrigin;
        [SerializeField, Min(0.1f)] private float probeRadius = 0.8f;
        [SerializeField] private LayerMask interactableLayers = ~0;

        private readonly Collider[] overlapResults = new Collider[16];
        private IInteractable currentTarget;

        /// <summary>Raised with the new target, or null when nothing is in reach.</summary>
        public event Action<IInteractable> TargetChanged;

        /// <summary>(target or null, whether anything happened)</summary>
        public event Action<IInteractable, bool> Interacted;

        public IInteractable CurrentTarget => currentTarget;

        public void RefreshTarget()
        {
            IInteractable nearest = FindNearestInteractable();
            if (nearest != currentTarget)
                ChangeTarget(nearest);
        }

        public void ClearTarget()
        {
            if (currentTarget != null)
                ChangeTarget(null);
        }

        public bool Interact(IIngredientHolder holder)
        {
            bool succeeded = currentTarget != null && currentTarget.Interact(holder);
            Interacted?.Invoke(currentTarget, succeeded);
            return succeeded;
        }

        private void ChangeTarget(IInteractable next)
        {
            SetFocus(currentTarget, false);
            currentTarget = next;
            SetFocus(currentTarget, true);
            TargetChanged?.Invoke(currentTarget);
        }

        private IInteractable FindNearestInteractable()
        {
            Vector3 origin = probeOrigin.position;
            int hitCount = Physics.OverlapSphereNonAlloc(origin, probeRadius, overlapResults, interactableLayers,
                QueryTriggerInteraction.Collide);

            IInteractable nearest = null;
            float nearestSqrDistance = float.MaxValue;
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = overlapResults[i];
                if (!hit.TryGetComponent(out IInteractable interactable))
                    continue;

                float sqrDistance = (hit.ClosestPoint(origin) - origin).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearest = interactable;
                }
            }

            return nearest;
        }

        private static void SetFocus(IInteractable target, bool isFocused)
        {
            if (target is IFocusable focusable)
                focusable.SetFocused(isFocused);
        }

        private void OnDrawGizmosSelected()
        {
            if (probeOrigin == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(probeOrigin.position, probeRadius);
        }
    }
}
