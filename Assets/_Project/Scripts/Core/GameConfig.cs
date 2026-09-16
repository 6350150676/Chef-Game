using UnityEngine;

namespace YesChef.Core
{
    /// <summary>
    /// Tunable round rules, kept in an asset so designers can balance the game without touching code.
    /// </summary>
    [CreateAssetMenu(menuName = "Yes Chef/Game Config", fileName = "GameConfig")]
    public sealed class GameConfig : ScriptableObject
    {
        [SerializeField, Min(1f)] private float roundDurationSeconds = 180f;
        [SerializeField, Min(0f)] private float orderRespawnDelaySeconds = 5f;

        public float RoundDurationSeconds => roundDurationSeconds;
        public float OrderRespawnDelaySeconds => orderRespawnDelaySeconds;
    }
}
