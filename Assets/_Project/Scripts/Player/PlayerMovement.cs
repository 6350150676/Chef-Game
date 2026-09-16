using UnityEngine;

namespace YesChef.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 6f;
        [SerializeField, Min(0f)] private float turnSpeedDegrees = 900f;

        private CharacterController characterController;

        /// <summary>0 when standing still, 1 at full speed.</summary>
        public float Speed01 { get; private set; }

        private void Awake() => characterController = GetComponent<CharacterController>();

        /// <summary>
        /// Moves across the floor. Input maps straight onto world X/Z because the camera is fixed and faces along +Z.
        /// </summary>
        public void Move(Vector2 input, float deltaTime)
        {
            var direction = new Vector3(input.x, 0f, input.y);
            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            Speed01 = direction.magnitude;
            characterController.Move(direction * (moveSpeed * deltaTime));

            if (Speed01 > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeedDegrees * deltaTime);
            }
        }

        public void Stop() => Speed01 = 0f;

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            // CharacterController overwrites direct transform changes while enabled.
            characterController.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            characterController.enabled = true;
            Speed01 = 0f;
        }
    }
}
