using System.Collections;
using System.Linq;
using NUnit.Framework;
using TinyVanguard.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class PlayerMovementPlayModeTests : InputTestFixture
    {
        private Keyboard _keyboard = null!;

        public override void Setup()
        {
            base.Setup();
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }

        [UnityTest]
        public IEnumerator WInputMovesAlongCameraForwardAndTurnsPlayer()
        {
            yield return SceneManager.LoadSceneAsync(
                "CombatSandbox",
                LoadSceneMode.Single);
            yield return null;

            var player = GameObject.FindWithTag("Player");
            var camera = Camera.main;

            Assert.That(player, Is.Not.Null);
            Assert.That(camera, Is.Not.Null);
            Assert.That(player.GetComponent<CharacterController>(), Is.Not.Null);
            var movementController = player.GetComponent<PlayerMovementController>();
            Assert.That(movementController, Is.Not.Null);
            var moveAction = movementController!.InputActions.FindAction(
                "Gameplay/Move",
                true);
            Assert.That(moveAction.enabled, Is.True);
            Assert.That(
                moveAction.controls.Any(control => control == _keyboard.wKey),
                Is.True,
                string.Join(", ", moveAction.controls.Select(control => control.path)));

            var expectedDirection = Vector3
                .ProjectOnPlane(camera!.transform.forward, Vector3.up)
                .normalized;
            var startPosition = player.transform.position;

            Press(_keyboard.wKey);
            yield return null;
            Assert.That(_keyboard.wKey.isPressed, Is.True);
            Assert.That(moveAction.ReadValue<Vector2>().y, Is.GreaterThan(0.9f));
            yield return new WaitForSeconds(0.25f);

            Release(_keyboard.wKey);
            yield return null;

            var horizontalDisplacement = Vector3.ProjectOnPlane(
                player.transform.position - startPosition,
                Vector3.up);

            Assert.That(horizontalDisplacement.magnitude, Is.GreaterThan(0.1f));
            Assert.That(
                Vector3.Dot(horizontalDisplacement.normalized, expectedDirection),
                Is.GreaterThan(0.98f));
            Assert.That(
                Vector3.Dot(player.transform.forward, horizontalDisplacement.normalized),
                Is.GreaterThan(0.95f));
        }
    }
}
