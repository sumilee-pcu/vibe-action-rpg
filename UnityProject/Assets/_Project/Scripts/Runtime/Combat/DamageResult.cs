namespace TinyVanguard.Combat
{
    public enum DamageRejectionReason
    {
        None,
        NonPositiveDamage,
        Invulnerable,
        AlreadyDead
    }

    public readonly struct DamageResult
    {
        public DamageResult(
            int requestedDamage,
            int appliedDamage,
            int previousHealth,
            int currentHealth,
            DamageRejectionReason rejectionReason,
            bool causedDeath)
        {
            RequestedDamage = requestedDamage;
            AppliedDamage = appliedDamage;
            PreviousHealth = previousHealth;
            CurrentHealth = currentHealth;
            RejectionReason = rejectionReason;
            CausedDeath = causedDeath;
        }

        public int RequestedDamage { get; }
        public int AppliedDamage { get; }
        public int PreviousHealth { get; }
        public int CurrentHealth { get; }
        public DamageRejectionReason RejectionReason { get; }
        public bool CausedDeath { get; }
        public bool WasApplied => AppliedDamage > 0;
    }
}
