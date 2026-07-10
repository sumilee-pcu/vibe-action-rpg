using System;

namespace TinyVanguard.Combat
{
    public sealed class HealthState
    {
        public HealthState(int maximumHealth)
        {
            if (maximumHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumHealth),
                    maximumHealth,
                    "Maximum health must be greater than zero.");
            }

            MaximumHealth = maximumHealth;
            CurrentHealth = maximumHealth;
        }

        public event Action<int, int> HealthChanged = delegate { };
        public event Action Died = delegate { };

        public int MaximumHealth { get; }
        public int CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth == 0;

        public DamageResult ApplyDamage(int damage, bool isInvulnerable = false)
        {
            var previousHealth = CurrentHealth;

            if (damage <= 0)
            {
                return Rejected(
                    damage,
                    previousHealth,
                    DamageRejectionReason.NonPositiveDamage);
            }

            if (IsDead)
            {
                return Rejected(
                    damage,
                    previousHealth,
                    DamageRejectionReason.AlreadyDead);
            }

            if (isInvulnerable)
            {
                return Rejected(
                    damage,
                    previousHealth,
                    DamageRejectionReason.Invulnerable);
            }

            var appliedDamage = Math.Min(damage, CurrentHealth);
            CurrentHealth -= appliedDamage;
            HealthChanged(previousHealth, CurrentHealth);

            var causedDeath = CurrentHealth == 0;
            if (causedDeath)
            {
                Died();
            }

            return new DamageResult(
                damage,
                appliedDamage,
                previousHealth,
                CurrentHealth,
                DamageRejectionReason.None,
                causedDeath);
        }

        public int Heal(int amount)
        {
            if (amount <= 0 || IsDead || CurrentHealth == MaximumHealth)
            {
                return 0;
            }

            var previousHealth = CurrentHealth;
            CurrentHealth = Math.Min(CurrentHealth + amount, MaximumHealth);
            var appliedHealing = CurrentHealth - previousHealth;
            HealthChanged(previousHealth, CurrentHealth);
            return appliedHealing;
        }

        private DamageResult Rejected(
            int requestedDamage,
            int previousHealth,
            DamageRejectionReason reason)
        {
            return new DamageResult(
                requestedDamage,
                0,
                previousHealth,
                CurrentHealth,
                reason,
                false);
        }
    }
}
