using System.Collections;
using System.Linq;
using NUnit.Framework;
using TinyVanguard.Session;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class GameplayInputGatePlayModeTests : InputTestFixture
    {
        private Keyboard _keyboard = null!;

        public override void Setup()
        {
            base.Setup();
            _keyboard = InputSystem.AddDevice<Keyboard>();
        }

        [UnityTest]
        public IEnumerator EscapeTogglesPauseAndPausedMovementIsBlocked()
        {
            yield return LoadCombatSandbox();

            var inputGate = Object.FindFirstObjectByType<GameplayInputGate>();
            var player = GameObject.FindWithTag("Player");
            Assert.That(inputGate, Is.Not.Null);
            Assert.That(player, Is.Not.Null);

            var gameplayMap = inputGate!.InputActions.FindActionMap("Gameplay", true);
            var systemMap = inputGate.InputActions.FindActionMap("System", true);
            var uiMap = inputGate.InputActions.FindActionMap("UI", true);
            AssertMapState(inputGate, GameSessionState.Playing, true, true, false);

            Press(_keyboard.escapeKey);
            yield return null;
            Release(_keyboard.escapeKey);
            yield return null;

            AssertMapState(inputGate, GameSessionState.Paused, false, true, true);
            Assert.That(gameplayMap.enabled, Is.False);
            Assert.That(systemMap.enabled, Is.True);
            Assert.That(uiMap.enabled, Is.True);

            var pausedStartPosition = player!.transform.position;
            Press(_keyboard.wKey);
            yield return null;
            yield return new WaitForSeconds(0.25f);
            Release(_keyboard.wKey);
            yield return null;

            var pausedDisplacement = Vector3.ProjectOnPlane(
                player.transform.position - pausedStartPosition,
                Vector3.up);
            Assert.That(pausedDisplacement.magnitude, Is.LessThan(0.01f));

            Press(_keyboard.escapeKey);
            yield return null;
            Release(_keyboard.escapeKey);
            yield return null;
            AssertMapState(inputGate, GameSessionState.Playing, true, true, false);

            var playingStartPosition = player.transform.position;
            Press(_keyboard.wKey);
            yield return null;
            yield return new WaitForSeconds(0.25f);
            Release(_keyboard.wKey);
            yield return null;

            var playingDisplacement = Vector3.ProjectOnPlane(
                player.transform.position - playingStartPosition,
                Vector3.up);
            Assert.That(playingDisplacement.magnitude, Is.GreaterThan(0.1f));
        }

        [UnityTest]
        public IEnumerator VictoryAndDefeatRejectPauseAndKeepGameplayDisabled()
        {
            yield return LoadCombatSandbox();

            var inputGate = Object.FindFirstObjectByType<GameplayInputGate>();
            Assert.That(inputGate, Is.Not.Null);

            Assert.That(inputGate!.SetState(GameSessionState.Victory), Is.True);
            AssertMapState(inputGate, GameSessionState.Victory, false, true, true);
            yield return PressAndReleaseEscape();
            AssertMapState(inputGate, GameSessionState.Victory, false, true, true);

            Assert.That(inputGate.SetState(GameSessionState.Defeat), Is.True);
            AssertMapState(inputGate, GameSessionState.Defeat, false, true, true);
            yield return PressAndReleaseEscape();
            AssertMapState(inputGate, GameSessionState.Defeat, false, true, true);

            Assert.That(
                inputGate.InputActions.FindActionMap("Gameplay", true)
                    .actions.All(action => !action.enabled),
                Is.True);
        }

        private IEnumerator PressAndReleaseEscape()
        {
            Press(_keyboard.escapeKey);
            yield return null;
            Release(_keyboard.escapeKey);
            yield return null;
        }

        private static IEnumerator LoadCombatSandbox()
        {
            yield return SceneManager.LoadSceneAsync(
                "CombatSandbox",
                LoadSceneMode.Single);
            yield return null;
        }

        private static void AssertMapState(
            GameplayInputGate inputGate,
            GameSessionState expectedState,
            bool gameplay,
            bool system,
            bool ui)
        {
            Assert.That(inputGate.State, Is.EqualTo(expectedState));
            Assert.That(
                inputGate.InputActions.FindActionMap("Gameplay", true).enabled,
                Is.EqualTo(gameplay));
            Assert.That(
                inputGate.InputActions.FindActionMap("System", true).enabled,
                Is.EqualTo(system));
            Assert.That(
                inputGate.InputActions.FindActionMap("UI", true).enabled,
                Is.EqualTo(ui));
        }
    }
}
