using System.Collections;
using NUnit.Framework;
using TinyVanguard.Enemies;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class EnemyNavigationPlayModeTests
    {
        [UnityTest]
        public IEnumerator MeleeGruntHasCompletePathAndMovesOnBakedNavMesh()
        {
            yield return SceneManager.LoadSceneAsync("CombatSandbox", LoadSceneMode.Single);
            yield return null;

            var enemy = GameObject.Find("MeleeGrunt");
            Assert.That(enemy, Is.Not.Null);
            var navigation = enemy!.GetComponent<EnemyNavigationController>();
            var brain = enemy.GetComponent<EnemyBrain>();
            if (brain != null)
            {
                brain.enabled = false;
            }
            Assert.That(navigation, Is.Not.Null);
            Assert.That(navigation!.Definition, Is.Not.Null);
            Assert.That(navigation.IsOnNavMesh, Is.True);
            navigation.Stop();
            Assert.That(
                navigation.Agent.speed,
                Is.EqualTo(navigation.Definition.MoveSpeed).Within(0.001f));
            Assert.That(
                navigation.Agent.stoppingDistance,
                Is.EqualTo(navigation.Definition.NavigationStoppingDistance).Within(0.001f));

            var start = enemy.transform.position;
            Assert.That(NavMesh.SamplePosition(
                start + Vector3.left * 2f,
                out var destination,
                1f,
                NavMesh.AllAreas), Is.True);
            Assert.That(navigation.TryMoveTo(destination.position), Is.True);

            var timeout = 1f;
            while (navigation.Agent.pathPending && timeout > 0f)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            Assert.That(navigation.Agent.pathStatus, Is.EqualTo(NavMeshPathStatus.PathComplete));
            yield return new WaitForSeconds(0.35f);
            Assert.That(Vector3.Distance(start, enemy.transform.position), Is.GreaterThan(0.25f));

            navigation.Stop();
            Assert.That(navigation.Agent.isStopped, Is.True);
        }
    }
}
