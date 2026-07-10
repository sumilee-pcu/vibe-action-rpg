using System;
using NUnit.Framework;
using TinyVanguard.Combat;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class HealthStateTests
    {
        [TestCase(0)]
        [TestCase(-1)]
        public void MaximumHealthMustBePositive(int maximumHealth)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new HealthState(maximumHealth));
        }

        [Test]
        public void NonLethalDamageReducesHealthAndReportsAppliedAmount()
        {
            var health = new HealthState(100);

            var result = health.ApplyDamage(30);

            Assert.That(result.WasApplied, Is.True);
            Assert.That(result.AppliedDamage, Is.EqualTo(30));
            Assert.That(result.PreviousHealth, Is.EqualTo(100));
            Assert.That(result.CurrentHealth, Is.EqualTo(70));
            Assert.That(result.CausedDeath, Is.False);
            Assert.That(health.IsDead, Is.False);
        }

        [Test]
        public void LethalDamageClampsHealthToZeroAndFiresDeathOnce()
        {
            var health = new HealthState(100);
            var deathCount = 0;
            var healthChangeCount = 0;
            health.Died += () => deathCount++;
            health.HealthChanged += (_, _) => healthChangeCount++;

            var lethal = health.ApplyDamage(150);
            var repeated = health.ApplyDamage(10);

            Assert.That(lethal.AppliedDamage, Is.EqualTo(100));
            Assert.That(lethal.CurrentHealth, Is.Zero);
            Assert.That(lethal.CausedDeath, Is.True);
            Assert.That(repeated.WasApplied, Is.False);
            Assert.That(repeated.RejectionReason,
                Is.EqualTo(DamageRejectionReason.AlreadyDead));
            Assert.That(repeated.CausedDeath, Is.False);
            Assert.That(health.CurrentHealth, Is.Zero);
            Assert.That(deathCount, Is.EqualTo(1));
            Assert.That(healthChangeCount, Is.EqualTo(1));
        }

        [Test]
        public void ExactCurrentHealthDamageIsLethal()
        {
            var health = new HealthState(40);

            var result = health.ApplyDamage(40);

            Assert.That(result.CausedDeath, Is.True);
            Assert.That(health.CurrentHealth, Is.Zero);
        }

        [Test]
        public void InvulnerabilityRejectsDamageWithoutEvents()
        {
            var health = new HealthState(100);
            var healthChangeCount = 0;
            health.HealthChanged += (_, _) => healthChangeCount++;

            var blocked = health.ApplyDamage(25, isInvulnerable: true);
            var applied = health.ApplyDamage(25, isInvulnerable: false);

            Assert.That(blocked.WasApplied, Is.False);
            Assert.That(blocked.RejectionReason,
                Is.EqualTo(DamageRejectionReason.Invulnerable));
            Assert.That(blocked.CurrentHealth, Is.EqualTo(100));
            Assert.That(applied.AppliedDamage, Is.EqualTo(25));
            Assert.That(health.CurrentHealth, Is.EqualTo(75));
            Assert.That(healthChangeCount, Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void NonPositiveDamageIsRejected(int damage)
        {
            var health = new HealthState(100);

            var result = health.ApplyDamage(damage);

            Assert.That(result.WasApplied, Is.False);
            Assert.That(result.RejectionReason,
                Is.EqualTo(DamageRejectionReason.NonPositiveDamage));
            Assert.That(health.CurrentHealth, Is.EqualTo(100));
        }

        [Test]
        public void HealingClampsAtMaximumHealth()
        {
            var health = new HealthState(100);
            health.ApplyDamage(30);

            var appliedHealing = health.Heal(50);

            Assert.That(appliedHealing, Is.EqualTo(30));
            Assert.That(health.CurrentHealth, Is.EqualTo(100));
        }

        [Test]
        public void HealingDoesNotReviveDeadActor()
        {
            var health = new HealthState(100);
            health.ApplyDamage(100);

            var appliedHealing = health.Heal(100);

            Assert.That(appliedHealing, Is.Zero);
            Assert.That(health.IsDead, Is.True);
        }
    }
}
