using System;

namespace TinyVanguard.Enemies
{
    public sealed class EnemyStateMachine
    {
        public EnemyStateMachine(EnemyState initialState = EnemyState.Idle)
        {
            if (!Enum.IsDefined(typeof(EnemyState), initialState))
            {
                throw new ArgumentOutOfRangeException(nameof(initialState));
            }

            CurrentState = initialState;
        }

        public event Action<EnemyStateTransition> StateChanged = delegate { };

        public EnemyState CurrentState { get; private set; }
        public EnemyStateTransition LastTransition { get; private set; }
        public int TransitionCount { get; private set; }
        public bool IsDead => CurrentState == EnemyState.Dead;

        public bool Tick(EnemyStateSignals signals)
        {
            if (CurrentState == EnemyState.Dead)
            {
                return false;
            }

            if (signals.IsDead)
            {
                return TransitionTo(EnemyState.Dead);
            }

            if (signals.ReceivedHit)
            {
                return TransitionTo(EnemyState.Hit);
            }

            switch (CurrentState)
            {
                case EnemyState.Idle:
                    return signals.HasLivingTarget
                        && TransitionTo(EnemyState.Chase);

                case EnemyState.Chase:
                    if (signals.ShouldDisengage || !signals.HasLivingTarget)
                    {
                        return TransitionTo(EnemyState.Return);
                    }

                    return signals.IsTargetInAttackRange
                        && signals.CanAttack
                        && TransitionTo(EnemyState.Attack);

                case EnemyState.Attack:
                    if (signals.ShouldDisengage || !signals.HasLivingTarget)
                    {
                        return TransitionTo(EnemyState.Return);
                    }

                    return signals.HasFinishedAttack
                        && TransitionTo(EnemyState.Chase);

                case EnemyState.Hit:
                    if (!signals.HasRecoveredFromHit)
                    {
                        return false;
                    }

                    if (signals.ShouldDisengage || !signals.HasLivingTarget)
                    {
                        return TransitionTo(EnemyState.Return);
                    }

                    return signals.IsTargetInAttackRange && signals.CanAttack
                        ? TransitionTo(EnemyState.Attack)
                        : TransitionTo(EnemyState.Chase);

                case EnemyState.Return:
                    return signals.IsAtHome
                        && TransitionTo(EnemyState.Idle);

                default:
                    throw new InvalidOperationException(
                        $"Unhandled enemy state: {CurrentState}");
            }
        }

        private bool TransitionTo(EnemyState nextState)
        {
            if (CurrentState == nextState)
            {
                return false;
            }

            LastTransition = new EnemyStateTransition(CurrentState, nextState);
            CurrentState = nextState;
            TransitionCount++;
            StateChanged(LastTransition);
            return true;
        }
    }
}
