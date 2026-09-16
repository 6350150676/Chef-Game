using UnityEngine;

namespace YesChef.Core
{
    /// <summary>
    /// Plain countdown advanced by explicit ticks. Owners feed it scaled delta time, so pausing via
    /// <see cref="Time.timeScale"/> freezes every timer in the game without extra bookkeeping.
    /// </summary>
    public sealed class CountdownTimer
    {
        public float Duration { get; private set; }
        public float Remaining { get; private set; }
        public bool IsRunning => Remaining > 0f;
        public float Progress01 => Duration <= 0f ? 1f : 1f - Remaining / Duration;

        public void Start(float duration)
        {
            Duration = Mathf.Max(0f, duration);
            Remaining = Duration;
        }

        public void Stop() => Remaining = 0f;

        /// <returns>True only on the tick that finishes the countdown.</returns>
        public bool Tick(float deltaTime)
        {
            if (!IsRunning)
                return false;

            Remaining = Mathf.Max(0f, Remaining - deltaTime);
            return !IsRunning;
        }
    }
}
