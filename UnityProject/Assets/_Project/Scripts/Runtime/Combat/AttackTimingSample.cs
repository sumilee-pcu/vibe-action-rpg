namespace TinyVanguard.Combat
{
    public readonly struct AttackTimingSample
    {
        public AttackTimingSample(
            int attackSequence,
            double attackStartedAt,
            double damageResolvedAt,
            int appliedTargetCount)
        {
            AttackSequence = attackSequence;
            AttackStartedAt = attackStartedAt;
            DamageResolvedAt = damageResolvedAt;
            AppliedTargetCount = appliedTargetCount;
        }

        public int AttackSequence { get; }
        public double AttackStartedAt { get; }
        public double DamageResolvedAt { get; }
        public int AppliedTargetCount { get; }
        public double DelaySeconds => DamageResolvedAt - AttackStartedAt;
    }
}
