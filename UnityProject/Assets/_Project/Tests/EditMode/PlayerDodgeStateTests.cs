using NUnit.Framework;
using TinyVanguard.Player;
using UnityEngine;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class PlayerDodgeStateTests
    {
        [Test]
        public void InvulnerabilityUsesConfiguredHalfOpenWindow()
        {
            var dodge = new PlayerDodgeState();
            Assert.That(dodge.TryStart(Vector3.forward, 0.35f, 0.05f, 0.25f), Is.True);
            Assert.That(dodge.IsInvulnerable, Is.False);

            dodge.Step(0.05f, 4f);
            Assert.That(dodge.IsInvulnerable, Is.True);

            dodge.Step(0.199f, 4f);
            Assert.That(dodge.IsInvulnerable, Is.True);

            dodge.Step(0.001f, 4f);
            Assert.That(dodge.IsInvulnerable, Is.False);
        }

        [Test]
        public void ActiveDodgeCannotRestart()
        {
            var dodge = new PlayerDodgeState();
            Assert.That(dodge.TryStart(Vector3.forward, 0.35f, 0.05f, 0.25f), Is.True);
            Assert.That(dodge.TryStart(Vector3.right, 0.35f, 0.05f, 0.25f), Is.False);
            Assert.That(dodge.Direction, Is.EqualTo(Vector3.forward));
        }

        [TestCase(30)]
        [TestCase(120)]
        public void TotalDistanceIsFrameRateIndependent(int framesPerSecond)
        {
            var dodge = new PlayerDodgeState();
            dodge.TryStart(Vector3.right, 0.35f, 0.05f, 0.25f);
            var distance = 0f;
            var deltaTime = 1f / framesPerSecond;

            while (dodge.IsActive)
            {
                distance += dodge.Step(deltaTime, 4f).magnitude;
            }

            Assert.That(distance, Is.EqualTo(4f).Within(0.0001f));
        }

        [Test]
        public void CancelEndsDodgeAndInvulnerability()
        {
            var dodge = new PlayerDodgeState();
            dodge.TryStart(Vector3.forward, 0.35f, 0f, 0.25f);
            Assert.That(dodge.IsInvulnerable, Is.True);

            dodge.Cancel();

            Assert.That(dodge.IsActive, Is.False);
            Assert.That(dodge.IsInvulnerable, Is.False);
            Assert.That(dodge.Step(0.1f, 4f), Is.EqualTo(Vector3.zero));
        }
    }
}
