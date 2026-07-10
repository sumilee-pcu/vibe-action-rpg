using System.Collections;
using NUnit.Framework;
using TinyVanguard.Combat;
using UnityEngine;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class ActorFeedbackPlayModeTests
    {
        [UnityTest]
        public IEnumerator AppliedDamage_EmitsFeedbackAndTransitionsToDeathOnce()
        {
            var definition = ScriptableObject.CreateInstance<ActorDefinition>();
            var actorObject = new GameObject("FeedbackActor");
            var health = actorObject.AddComponent<ActorHealth>();
            health.Configure(definition);
            var action = actorObject.AddComponent<DeathDisabledActionProbe>();
            var reactions = actorObject.AddComponent<ActorReactionController>();
            reactions.Configure(health, disableOnDeath: new Behaviour[] { action });

            var damageEventCount = 0;
            var deathEventCount = 0;
            var lastDamage = default(DamageAppliedEvent);
            health.DamageApplied += damageEvent =>
            {
                damageEventCount++;
                lastDamage = damageEvent;
            };
            health.Died += actor => deathEventCount++;

            var hitPosition = new Vector3(1f, 2f, 3f);
            var firstResult = health.ApplyDamage(15, hitPosition);

            Assert.That(firstResult.WasApplied, Is.True);
            Assert.That(damageEventCount, Is.EqualTo(1));
            Assert.That(lastDamage.Target, Is.SameAs(health));
            Assert.That(lastDamage.Amount, Is.EqualTo(15));
            Assert.That(lastDamage.WorldPosition, Is.EqualTo(hitPosition));
            Assert.That(lastDamage.CausedDeath, Is.False);
            Assert.That(reactions.HitReactionCount, Is.EqualTo(1));
            Assert.That(reactions.IsDead, Is.False);

            var lethalResult = health.ApplyDamage(health.State.CurrentHealth, hitPosition);

            Assert.That(lethalResult.CausedDeath, Is.True);
            Assert.That(damageEventCount, Is.EqualTo(2));
            Assert.That(deathEventCount, Is.EqualTo(1));
            Assert.That(reactions.HitReactionCount, Is.EqualTo(1));
            Assert.That(reactions.DeathTransitionCount, Is.EqualTo(1));
            Assert.That(reactions.IsDead, Is.True);
            Assert.That(health.CanAct, Is.False);
            Assert.That(action.enabled, Is.False);

            var rejectedResult = health.ApplyDamage(1, hitPosition);

            Assert.That(rejectedResult.WasApplied, Is.False);
            Assert.That(damageEventCount, Is.EqualTo(2));
            Assert.That(deathEventCount, Is.EqualTo(1));
            Assert.That(reactions.DeathTransitionCount, Is.EqualTo(1));

            Object.Destroy(actorObject);
            Object.Destroy(definition);
            yield return null;
        }
    }

    public sealed class DeathDisabledActionProbe : MonoBehaviour
    {
    }
}
