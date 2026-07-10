namespace TinyVanguard.Enemies
{
    public readonly struct EnemyStateSignals
    {
        public EnemyStateSignals(
            bool isDead = false,
            bool hasLivingTarget = false,
            bool isTargetInAttackRange = false,
            bool canAttack = false,
            bool shouldDisengage = false,
            bool isAtHome = false,
            bool receivedHit = false,
            bool hasRecoveredFromHit = false,
            bool hasFinishedAttack = false)
        {
            IsDead = isDead;
            HasLivingTarget = hasLivingTarget;
            IsTargetInAttackRange = isTargetInAttackRange;
            CanAttack = canAttack;
            ShouldDisengage = shouldDisengage;
            IsAtHome = isAtHome;
            ReceivedHit = receivedHit;
            HasRecoveredFromHit = hasRecoveredFromHit;
            HasFinishedAttack = hasFinishedAttack;
        }

        public bool IsDead { get; }
        public bool HasLivingTarget { get; }
        public bool IsTargetInAttackRange { get; }
        public bool CanAttack { get; }
        public bool ShouldDisengage { get; }
        public bool IsAtHome { get; }
        public bool ReceivedHit { get; }
        public bool HasRecoveredFromHit { get; }
        public bool HasFinishedAttack { get; }
    }
}
