using System.Collections;
using NUnit.Framework;
using TinyVanguard.Combat;
using UnityEngine;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class MultiColliderAttackPlayModeTests
    {
        [UnityTest]
        public IEnumerator MultipleCollidersOnOneActorApplyDamageOnce()
        {
            var definition = ScriptableObject.CreateInstance<ActorDefinition>();
            var target = new GameObject("Multi Collider Target");
            target.SetActive(false);
            var health = target.AddComponent<ActorHealth>();
            health.Configure(definition);
            var damageEventCount = 0;
            health.DamageApplied += damageEvent => damageEventCount++;
            CreateCollider(target.transform, "Body Collider", Vector3.zero);
            CreateCollider(target.transform, "Weapon Collider", Vector3.right * 0.2f);
            target.SetActive(true);
            Physics.SyncTransforms();
            yield return null;

            var colliders = target.GetComponentsInChildren<Collider>();
            Assert.That(colliders.Length, Is.EqualTo(2));
            var execution = new AttackExecution(1);
            var appliedTargets = AttackHitResolver.ApplyDamage(colliders, execution, 15);

            Assert.That(appliedTargets, Is.EqualTo(1));
            Assert.That(health.State.CurrentHealth, Is.EqualTo(85));
            Assert.That(execution.RegisteredTargetCount, Is.EqualTo(1));
            Assert.That(damageEventCount, Is.EqualTo(1));

            var secondExecution = new AttackExecution(2);
            var secondApplied = AttackHitResolver.ApplyDamage(
                colliders,
                secondExecution,
                15);
            Assert.That(secondApplied, Is.EqualTo(1));
            Assert.That(health.State.CurrentHealth, Is.EqualTo(70));
            Assert.That(damageEventCount, Is.EqualTo(2));

            Object.Destroy(target);
            Object.Destroy(definition);
            yield return null;
        }

        private static void CreateCollider(
            Transform parent,
            string name,
            Vector3 localPosition)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent, false);
            child.transform.localPosition = localPosition;
            child.AddComponent<BoxCollider>();
        }
    }
}
