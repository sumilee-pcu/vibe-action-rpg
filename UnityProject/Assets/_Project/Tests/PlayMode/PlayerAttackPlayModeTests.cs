using System.Collections;
using NUnit.Framework;
using TinyVanguard.Combat;
using TinyVanguard.Session;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class PlayerAttackPlayModeTests : InputTestFixture
    {
        private Mouse _mouse = null!;

        public override void Setup()
        {
            base.Setup();
            _mouse = InputSystem.AddDevice<Mouse>();
        }

        [UnityTest]
        public IEnumerator MouseAttackOpensAndClosesConfiguredHitWindow()
        {
            yield return LoadCombatSandbox();
            var attack = Object.FindFirstObjectByType<PlayerAttackController>();
            var trainingTarget = GameObject.Find("TrainingTarget")?.GetComponent<ActorHealth>();
            Assert.That(attack, Is.Not.Null);
            Assert.That(trainingTarget, Is.Not.Null);

            var timingEventCount = 0;
            var timing = default(AttackTimingSample);
            attack!.AttackResolved += sample =>
            {
                timingEventCount++;
                timing = sample;
            };

            Press(_mouse.leftButton);
            yield return null;
            Release(_mouse.leftButton);
            yield return new WaitForSeconds(0.2f);
            Assert.That(attack.IsAttackInProgress, Is.True);
            Assert.That(attack.IsHitWindowActive, Is.True);
            Assert.That(timingEventCount, Is.EqualTo(1));
            Assert.That(timing.AppliedTargetCount, Is.EqualTo(1));
            Assert.That(trainingTarget!.State.CurrentHealth, Is.EqualTo(85));
            Assert.That(timing.DelaySeconds, Is.InRange(0.10d, 0.25d));
            Assert.That(
                timing.DelaySeconds,
                Is.EqualTo(attack.AttackDefinition.ActiveStartTime).Within(0.10d));
            TestContext.WriteLine(
                $"Attack timing: sequence={timing.AttackSequence}, "
                + $"delay={timing.DelaySeconds:F4}s, "
                + $"configured={attack.AttackDefinition.ActiveStartTime:F4}s, "
                + $"targets={timing.AppliedTargetCount}");

            yield return new WaitForSeconds(0.2f);
            Assert.That(attack.IsHitWindowActive, Is.False);

            yield return new WaitForSeconds(0.2f);
            Assert.That(attack.IsAttackInProgress, Is.False);
            Assert.That(attack.AttackSequence, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator PauseCancelsAttackAndClosesHitWindow()
        {
            yield return LoadCombatSandbox();
            var attack = Object.FindFirstObjectByType<PlayerAttackController>();
            var inputGate = Object.FindFirstObjectByType<GameplayInputGate>();
            Assert.That(attack, Is.Not.Null);
            Assert.That(inputGate, Is.Not.Null);

            Assert.That(attack!.TryStartAttack(), Is.True);
            attack.OpenHitWindow();
            Assert.That(attack.IsHitWindowActive, Is.True);

            inputGate!.SetState(GameSessionState.Paused);
            yield return null;

            Assert.That(attack.IsAttackInProgress, Is.False);
            Assert.That(attack.IsHitWindowActive, Is.False);
            Assert.That(attack.TryStartAttack(), Is.False);
        }

        private static IEnumerator LoadCombatSandbox()
        {
            yield return SceneManager.LoadSceneAsync("CombatSandbox", LoadSceneMode.Single);
            yield return null;
        }
    }
}
