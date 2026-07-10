using System.Linq;
using NUnit.Framework;
using TinyVanguard.Combat;
using UnityEditor;
using UnityEngine;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class PlayerAttackAnimationAssetTests
    {
        [Test]
        public void AnimationEventsMatchAttackDefinitionWindow()
        {
            var definition = AssetDatabase.LoadAssetAtPath<AttackDefinition>(
                "Assets/_Project/Data/Attacks/BasicAttack.asset");
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
                "Assets/_Project/Art/Animations/BasicAttack.anim");
            Assert.That(definition, Is.Not.Null);
            Assert.That(clip, Is.Not.Null);

            var events = AnimationUtility.GetAnimationEvents(clip!);
            Assert.That(events.Select(item => item.functionName), Is.EqualTo(new[]
            {
                nameof(PlayerAttackController.OpenHitWindow),
                nameof(PlayerAttackController.CloseHitWindow),
                nameof(PlayerAttackController.CompleteAttack)
            }));
            Assert.That(events[0].time, Is.EqualTo(definition!.ActiveStartTime).Within(0.0001f));
            Assert.That(events[1].time, Is.EqualTo(definition.ActiveEndTime).Within(0.0001f));
            Assert.That(events[2].time, Is.EqualTo(definition.Cooldown).Within(0.0001f));
        }
    }
}
