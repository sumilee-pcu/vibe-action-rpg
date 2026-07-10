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
            Assert.That(attack, Is.Not.Null);

            Press(_mouse.leftButton);
            yield return null;
            Release(_mouse.leftButton);
            yield return new WaitForSeconds(0.2f);
            Assert.That(attack!.IsAttackInProgress, Is.True);
            Assert.That(attack.IsHitWindowActive, Is.True);

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
