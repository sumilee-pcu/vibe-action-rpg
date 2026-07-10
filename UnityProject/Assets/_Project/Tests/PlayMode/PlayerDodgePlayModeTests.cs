using System.Collections;
using NUnit.Framework;
using TinyVanguard.Player;
using TinyVanguard.Session;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class PlayerDodgePlayModeTests : InputTestFixture
    {
        private Keyboard _keyboard = null!;

        public override void Setup()
        {
            base.Setup();
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }

        [UnityTest]
        public IEnumerator SpaceMovesPlayerAndExposesInvulnerabilityWindow()
        {
            yield return LoadCombatSandbox();
            var player = GameObject.FindWithTag("Player");
            var controller = player!.GetComponent<PlayerMovementController>();
            Assert.That(controller, Is.Not.Null);
            var startPosition = player.transform.position;

            Press(_keyboard.spaceKey);
            yield return null;
            Release(_keyboard.spaceKey);
            yield return new WaitForSeconds(0.1f);

            Assert.That(controller!.IsDodging, Is.True);
            Assert.That(controller.IsInvulnerable, Is.True);

            yield return new WaitForSeconds(0.35f);
            var displacement = Vector3.ProjectOnPlane(
                player.transform.position - startPosition,
                Vector3.up);
            Assert.That(controller.IsDodging, Is.False);
            Assert.That(controller.IsInvulnerable, Is.False);
            Assert.That(displacement.magnitude, Is.EqualTo(4f).Within(0.2f));
        }

        [UnityTest]
        public IEnumerator PauseCancelsActiveDodgeAndRejectsNewDodgeInput()
        {
            yield return LoadCombatSandbox();
            var player = GameObject.FindWithTag("Player");
            var controller = player!.GetComponent<PlayerMovementController>();
            var inputGate = Object.FindFirstObjectByType<GameplayInputGate>();
            Assert.That(controller, Is.Not.Null);
            Assert.That(inputGate, Is.Not.Null);

            Press(_keyboard.spaceKey);
            yield return null;
            Release(_keyboard.spaceKey);
            yield return null;
            Assert.That(controller!.IsDodging, Is.True);

            inputGate!.SetState(GameSessionState.Paused);
            yield return null;
            Assert.That(controller.IsDodging, Is.False);
            var pausedPosition = player.transform.position;

            Press(_keyboard.spaceKey);
            yield return null;
            Release(_keyboard.spaceKey);
            yield return new WaitForSeconds(0.4f);

            var displacement = Vector3.ProjectOnPlane(
                player.transform.position - pausedPosition,
                Vector3.up);
            Assert.That(controller.IsDodging, Is.False);
            Assert.That(displacement.magnitude, Is.LessThan(0.01f));
        }

        private static IEnumerator LoadCombatSandbox()
        {
            yield return SceneManager.LoadSceneAsync("CombatSandbox", LoadSceneMode.Single);
            yield return null;
        }
    }
}
