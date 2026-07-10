using System.Collections;
using NUnit.Framework;
using TinyVanguard.Combat;
using TinyVanguard.Enemies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class EnemyCombatPlayModeTests
    {
        [UnityTest]
        public IEnumerator MeleeGruntDetectsPursuesStopsAndRespectsAttackCooldown()
        {
            yield return SceneManager.LoadSceneAsync("CombatSandbox", LoadSceneMode.Single);
            yield return null;

            var enemy = GameObject.Find("MeleeGrunt");
            var player = GameObject.FindGameObjectWithTag("Player");
            Assert.That(enemy, Is.Not.Null);
            Assert.That(player, Is.Not.Null);

            var brain = enemy!.GetComponent<EnemyBrain>();
            var playerHealth = player!.GetComponent<ActorHealth>();
            Assert.That(brain, Is.Not.Null);
            Assert.That(playerHealth, Is.Not.Null);
            Assert.That(brain!.CurrentState, Is.EqualTo(EnemyState.Chase));
            Assert.That(brain.AgentIsMoving(), Is.True);

            var attackStateCount = 0;
            brain.StateChanged += transition =>
            {
                if (transition.Current == EnemyState.Attack)
                {
                    attackStateCount++;
                }
            };

            var attackPosition = player.transform.position + Vector3.right * 1.4f;
            Assert.That(brain.GetComponent<EnemyNavigationController>().Agent.Warp(
                attackPosition), Is.True);
            yield return null;

            Assert.That(brain.AttackExecutionCount, Is.EqualTo(1));
            Assert.That(brain.AppliedAttackCount, Is.EqualTo(1));
            Assert.That(attackStateCount, Is.EqualTo(1));
            Assert.That(playerHealth!.State.CurrentHealth, Is.EqualTo(84));
            Assert.That(brain.GetComponent<EnemyNavigationController>().Agent.isStopped, Is.True);

            yield return new WaitForSeconds(0.25f);
            Assert.That(brain.AttackExecutionCount, Is.EqualTo(1));
            Assert.That(playerHealth.State.CurrentHealth, Is.EqualTo(84));

            yield return new WaitForSeconds(brain.Definition.AttackCooldown + 0.1f);
            Assert.That(brain.AttackExecutionCount, Is.EqualTo(2));
            Assert.That(brain.AppliedAttackCount, Is.EqualTo(2));
            Assert.That(playerHealth.State.CurrentHealth, Is.EqualTo(68));
        }
    }

    internal static class EnemyBrainTestExtensions
    {
        public static bool AgentIsMoving(this EnemyBrain brain)
        {
            var agent = brain.GetComponent<EnemyNavigationController>().Agent;
            return agent.hasPath && !agent.isStopped;
        }
    }
}
