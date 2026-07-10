using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TinyVanguard.Combat;
using UnityEditor;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class CombatDefinitionTests
    {
        private static readonly string[] ForbiddenRuntimeStateNames =
        {
            "current",
            "remaining",
            "runtime",
            "elapsed",
            "isdead",
            "hittarget"
        };

        [Test]
        public void DefaultActorDefinitionContainsOnlyTuningValues()
        {
            var actor = AssetDatabase.LoadAssetAtPath<ActorDefinition>(
                "Assets/_Project/Data/Actors/Player.asset");

            Assert.That(actor, Is.Not.Null);
            Assert.That(actor!.Identifier, Is.EqualTo("player"));
            Assert.That(actor.MaximumHealth, Is.EqualTo(100));
            Assert.That(actor.AttackPower, Is.EqualTo(10));
            Assert.That(actor.MoveSpeed, Is.EqualTo(5f));
            AssertHasNoRuntimeStateFields(typeof(ActorDefinition));
        }

        [Test]
        public void DefaultAttackDefinitionHasValidTimingAndTuning()
        {
            var attack = AssetDatabase.LoadAssetAtPath<AttackDefinition>(
                "Assets/_Project/Data/Attacks/BasicAttack.asset");

            Assert.That(attack, Is.Not.Null);
            Assert.That(attack!.Identifier, Is.EqualTo("basic-attack"));
            Assert.That(attack.BaseDamage, Is.EqualTo(15));
            Assert.That(attack.ActiveStartTime, Is.LessThan(attack.ActiveEndTime));
            Assert.That(attack.Cooldown, Is.GreaterThanOrEqualTo(attack.ActiveEndTime));
            Assert.That(attack.MovementMultiplier, Is.InRange(0f, 1f));
            AssertHasNoRuntimeStateFields(typeof(AttackDefinition));
        }

        private static void AssertHasNoRuntimeStateFields(Type definitionType)
        {
            var fields = definitionType.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var forbidden = fields
                .Select(field => field.Name.ToLowerInvariant())
                .Where(name => ForbiddenRuntimeStateNames.Any(name.Contains))
                .ToArray();

            Assert.That(
                forbidden,
                Is.Empty,
                $"{definitionType.Name} must not serialize runtime state.");
        }
    }
}
