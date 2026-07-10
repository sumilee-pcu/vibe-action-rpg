using System.Collections;
using NUnit.Framework;
using TinyVanguard.Enemies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class EnemyReturnPlayModeTests
    {
        [UnityTest]
        public IEnumerator PlayerBeyondHomeDisengageRangeReturnsEnemyToIdle()
        {
            yield return SceneManager.LoadSceneAsync("CombatSandbox", LoadSceneMode.Single);
            yield return null;

            var enemy = GameObject.Find("MeleeGrunt");
            var player = GameObject.FindGameObjectWithTag("Player");
            Assert.That(enemy, Is.Not.Null);
            Assert.That(player, Is.Not.Null);

            var brain = enemy!.GetComponent<EnemyBrain>();
            var navigation = enemy.GetComponent<EnemyNavigationController>();
            Assert.That(brain, Is.Not.Null);
            Assert.That(navigation, Is.Not.Null);
            Assert.That(brain!.CurrentState, Is.EqualTo(EnemyState.Chase));

            player!.transform.position = new Vector3(20f, 0f, 0f);
            yield return null;

            Assert.That(brain.CurrentState, Is.EqualTo(EnemyState.Return));
            Assert.That(navigation!.Agent.isStopped, Is.False);
            Assert.That(
                navigation.Agent.stoppingDistance,
                Is.EqualTo(brain.Definition.HomeTolerance).Within(0.001f));
            Assert.That(
                Vector3.Distance(navigation.Agent.destination, brain.HomePosition),
                Is.LessThan(brain.Definition.HomeTolerance));

            Assert.That(navigation.Agent.Warp(brain.HomePosition), Is.True);
            yield return null;

            Assert.That(brain.CurrentState, Is.EqualTo(EnemyState.Idle));
            Assert.That(navigation.Agent.isStopped, Is.True);
            Assert.That(brain.AttackExecutionCount, Is.EqualTo(0));
        }
    }
}
