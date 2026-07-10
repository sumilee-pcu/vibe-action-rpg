using NUnit.Framework;
using TinyVanguard.Enemies;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class EnemyStateMachineTests
    {
        [Test]
        public void LivingTargetTransitionsIdleToChaseToAttack()
        {
            var stateMachine = new EnemyStateMachine();

            Assert.That(stateMachine.Tick(new EnemyStateSignals(
                hasLivingTarget: true)), Is.True);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Chase));

            Assert.That(stateMachine.Tick(new EnemyStateSignals(
                hasLivingTarget: true,
                isTargetInAttackRange: true,
                canAttack: true)), Is.True);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Attack));
            Assert.That(stateMachine.TransitionCount, Is.EqualTo(2));
        }

        [Test]
        public void DisengageTransitionsToReturnAndHomeTransitionsToIdle()
        {
            var stateMachine = CreateChasingStateMachine();

            stateMachine.Tick(new EnemyStateSignals(
                hasLivingTarget: true,
                shouldDisengage: true));
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Return));

            Assert.That(stateMachine.Tick(new EnemyStateSignals(
                isAtHome: false)), Is.False);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Return));

            Assert.That(stateMachine.Tick(new EnemyStateSignals(
                isAtHome: true)), Is.True);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Idle));
        }

        [Test]
        public void HitInterruptsChaseAndWaitsForRecovery()
        {
            var stateMachine = CreateChasingStateMachine();

            stateMachine.Tick(new EnemyStateSignals(receivedHit: true));
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Hit));

            Assert.That(stateMachine.Tick(new EnemyStateSignals(
                hasLivingTarget: true)), Is.False);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Hit));

            stateMachine.Tick(new EnemyStateSignals(
                hasLivingTarget: true,
                hasRecoveredFromHit: true));
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Chase));
        }

        [Test]
        public void AttackWaitsForCompletionBeforeReturningToChase()
        {
            var stateMachine = CreateAttackingStateMachine();

            Assert.That(stateMachine.Tick(new EnemyStateSignals(
                hasLivingTarget: true,
                hasFinishedAttack: false)), Is.False);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Attack));

            Assert.That(stateMachine.Tick(new EnemyStateSignals(
                hasLivingTarget: true,
                hasFinishedAttack: true)), Is.True);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Chase));
        }

        [Test]
        public void DeadIsHighestPriorityAndTerminal()
        {
            var stateMachine = CreateAttackingStateMachine();
            var eventCount = 0;
            var lastTransition = default(EnemyStateTransition);
            stateMachine.StateChanged += transition =>
            {
                eventCount++;
                lastTransition = transition;
            };

            Assert.That(stateMachine.Tick(new EnemyStateSignals(
                isDead: true,
                receivedHit: true,
                hasLivingTarget: true)), Is.True);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Dead));
            Assert.That(lastTransition.Previous, Is.EqualTo(EnemyState.Attack));
            Assert.That(lastTransition.Current, Is.EqualTo(EnemyState.Dead));
            Assert.That(eventCount, Is.EqualTo(1));

            Assert.That(stateMachine.Tick(new EnemyStateSignals(
                receivedHit: true,
                hasLivingTarget: true,
                isTargetInAttackRange: true,
                canAttack: true)), Is.False);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(EnemyState.Dead));
            Assert.That(eventCount, Is.EqualTo(1));
        }

        private static EnemyStateMachine CreateChasingStateMachine()
        {
            var stateMachine = new EnemyStateMachine();
            stateMachine.Tick(new EnemyStateSignals(hasLivingTarget: true));
            return stateMachine;
        }

        private static EnemyStateMachine CreateAttackingStateMachine()
        {
            var stateMachine = CreateChasingStateMachine();
            stateMachine.Tick(new EnemyStateSignals(
                hasLivingTarget: true,
                isTargetInAttackRange: true,
                canAttack: true));
            return stateMachine;
        }
    }
}
