using System.Collections;
using NUnit.Framework;
using TinyVanguard.Combat;
using TinyVanguard.Enemies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class EnemyDeathPlayModeTests
    {
        [UnityTest]
        public IEnumerator LethalDamageStopsActionsAndSignalsRewardOnce()
        {
            yield return SceneManager.LoadSceneAsync("CombatSandbox", LoadSceneMode.Single);
            yield return null;

            var enemy = GameObject.Find("MeleeGrunt");
            Assert.That(enemy, Is.Not.Null);
            var brain = enemy!.GetComponent<EnemyBrain>();
            var health = enemy.GetComponent<ActorHealth>();
            var navigation = enemy.GetComponent<EnemyNavigationController>();
            Assert.That(brain, Is.Not.Null);
            Assert.That(health, Is.Not.Null);
            Assert.That(navigation, Is.Not.Null);

            var rewardEventCount = 0;
            var rewardAmount = 0;
            brain!.RewardAvailable += reward =>
            {
                rewardEventCount++;
                rewardAmount = reward;
            };
            var attacksBeforeDeath = brain.AttackExecutionCount;

            var lethalResult = health!.ApplyDamage(health.State.CurrentHealth);

            Assert.That(lethalResult.CausedDeath, Is.True);
            Assert.That(brain.CurrentState, Is.EqualTo(EnemyState.Dead));
            Assert.That(navigation!.Agent.isStopped, Is.True);
            Assert.That(brain.DeathHandledCount, Is.EqualTo(1));
            Assert.That(brain.RewardSignalCount, Is.EqualTo(1));
            Assert.That(rewardEventCount, Is.EqualTo(1));
            Assert.That(rewardAmount, Is.EqualTo(brain.Definition.ExperienceReward));

            var repeatedResult = health.ApplyDamage(1);
            brain.Tick(Time.timeAsDouble + 100d);
            yield return null;

            Assert.That(repeatedResult.WasApplied, Is.False);
            Assert.That(brain.CurrentState, Is.EqualTo(EnemyState.Dead));
            Assert.That(brain.AttackExecutionCount, Is.EqualTo(attacksBeforeDeath));
            Assert.That(brain.DeathHandledCount, Is.EqualTo(1));
            Assert.That(brain.RewardSignalCount, Is.EqualTo(1));
            Assert.That(rewardEventCount, Is.EqualTo(1));
        }
    }
}
