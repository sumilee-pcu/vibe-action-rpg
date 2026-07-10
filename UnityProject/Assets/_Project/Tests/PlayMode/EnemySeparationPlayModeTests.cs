using System.Collections;
using System.Linq;
using NUnit.Framework;
using TinyVanguard.Combat;
using TinyVanguard.Enemies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class EnemySeparationPlayModeTests
    {
        [UnityTest]
        public IEnumerator FiveEnemiesApproachDistinctSlotsWithoutCompleteOverlap()
        {
            yield return SceneManager.LoadSceneAsync("CombatSandbox", LoadSceneMode.Single);
            yield return null;

            var player = GameObject.FindGameObjectWithTag("Player");
            var playerHealth = player.GetComponent<ActorHealth>();
            var brains = Object.FindObjectsByType<EnemyBrain>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .OrderBy(brain => brain.name)
                .ToArray();
            Assert.That(brains.Length, Is.EqualTo(5));
            var startPositions = brains
                .Select(brain => brain.transform.position)
                .ToArray();

            foreach (var brain in brains)
            {
                brain.enabled = true;
            }

            var timeout = 3f;
            while (timeout > 0f && brains.Any(brain =>
                       Vector3.Distance(brain.transform.position, player.transform.position)
                       >= 2.25f))
            {
                playerHealth.State.Heal(playerHealth.State.MaximumHealth);
                timeout -= Time.deltaTime;
                yield return null;
            }

            for (var first = 0; first < brains.Length; first++)
            {
                var navigation = brains[first].GetComponent<EnemyNavigationController>();
                TestContext.WriteLine(
                    $"{brains[first].name}: "
                    + $"moved={Vector3.Distance(startPositions[first], brains[first].transform.position):F3}, "
                    + $"targetDistance={Vector3.Distance(brains[first].transform.position, player.transform.position):F3}, "
                    + $"onNavMesh={navigation.IsOnNavMesh}, "
                    + $"path={navigation.Agent.pathStatus}, "
                    + $"state={brains[first].CurrentState}");
                Assert.That(
                    Vector3.Distance(brains[first].transform.position, player.transform.position),
                    Is.LessThan(2.25f));
                for (var second = first + 1; second < brains.Length; second++)
                {
                    Assert.That(
                        Vector3.Distance(
                            brains[first].transform.position,
                            brains[second].transform.position),
                        Is.GreaterThan(0.5f));
                }
            }
        }
    }
}
