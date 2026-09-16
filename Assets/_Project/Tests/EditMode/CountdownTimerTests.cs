using NUnit.Framework;
using YesChef.Core;

namespace YesChef.Tests
{
    public class CountdownTimerTests
    {
        [Test]
        public void Tick_ReportsFinishOnlyOnTheFinishingTick()
        {
            var timer = new CountdownTimer();
            timer.Start(1f);

            Assert.IsFalse(timer.Tick(0.6f));
            Assert.IsTrue(timer.Tick(0.6f));
            Assert.IsFalse(timer.Tick(0.6f));
            Assert.AreEqual(0f, timer.Remaining);
        }

        [Test]
        public void Progress01_TracksElapsedFraction()
        {
            var timer = new CountdownTimer();
            timer.Start(4f);
            timer.Tick(1f);

            Assert.AreEqual(0.25f, timer.Progress01, 1e-5f);
        }

        [Test]
        public void Stop_EndsTheCountdownWithoutFinishing()
        {
            var timer = new CountdownTimer();
            timer.Start(5f);
            timer.Stop();

            Assert.IsFalse(timer.IsRunning);
            Assert.IsFalse(timer.Tick(1f));
        }
    }
}
