using System.Collections;
using NUnit.Framework;
using TinyVanguard.CameraControl;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TinyVanguard.Tests.PlayMode
{
    public sealed class ThirdPersonCameraPlayModeTests : InputTestFixture
    {
        private Mouse _mouse = null!;

        public override void Setup()
        {
            base.Setup();
            _mouse = InputSystem.AddDevice<Mouse>();
        }

        [UnityTest]
        public IEnumerator MouseDeltaRotatesConfiguredCinemachineOrbitWithinLimits()
        {
            yield return SceneManager.LoadSceneAsync(
                "CombatSandbox",
                LoadSceneMode.Single);
            yield return null;

            var player = GameObject.FindWithTag("Player");
            var camera = Camera.main;
            var cinemachineCamera = Object.FindFirstObjectByType<CinemachineCamera>();
            var orbitalFollow = Object.FindFirstObjectByType<CinemachineOrbitalFollow>();
            var rotationComposer = Object.FindFirstObjectByType<CinemachineRotationComposer>();
            var controller = Object.FindFirstObjectByType<ThirdPersonCameraController>();

            Assert.That(player, Is.Not.Null);
            Assert.That(camera, Is.Not.Null);
            Assert.That(camera!.GetComponent<CinemachineBrain>(), Is.Not.Null);
            Assert.That(cinemachineCamera, Is.Not.Null);
            Assert.That(orbitalFollow, Is.Not.Null);
            Assert.That(rotationComposer, Is.Not.Null);
            Assert.That(controller, Is.Not.Null);

            var cameraTarget = player!.transform.Find("Camera Target");
            Assert.That(cameraTarget, Is.Not.Null);
            Assert.That(cinemachineCamera!.Follow, Is.EqualTo(cameraTarget));
            Assert.That(cinemachineCamera.LookAt, Is.EqualTo(cameraTarget));
            Assert.That(orbitalFollow!.OrbitStyle,
                Is.EqualTo(CinemachineOrbitalFollow.OrbitStyles.Sphere));
            Assert.That(orbitalFollow.HorizontalAxis.Wrap, Is.True);
            Assert.That(orbitalFollow.VerticalAxis.Wrap, Is.False);
            Assert.That(orbitalFollow.VerticalAxis.Range,
                Is.EqualTo(new Vector2(-20f, 65f)));
            Assert.That(controller!.Sensitivity,
                Is.EqualTo(new Vector2(0.12f, 0.08f)));

            var startHorizontal = orbitalFollow.HorizontalAxis.Value;
            Set(_mouse.delta, new Vector2(100f, 1000f));
            yield return null;

            Assert.That(orbitalFollow.HorizontalAxis.Value,
                Is.EqualTo(startHorizontal + 12f).Within(0.001f));
            Assert.That(orbitalFollow.VerticalAxis.Value, Is.EqualTo(65f));
        }

        [UnityTest]
        public IEnumerator OutputCameraFollowsPlayerTarget()
        {
            yield return SceneManager.LoadSceneAsync(
                "CombatSandbox",
                LoadSceneMode.Single);
            yield return null;
            yield return null;

            var player = GameObject.FindWithTag("Player");
            var camera = Camera.main;
            Assert.That(player, Is.Not.Null);
            Assert.That(camera, Is.Not.Null);

            var startCameraPosition = camera!.transform.position;
            player!.transform.position += Vector3.right * 3f;
            yield return new WaitForSeconds(0.3f);

            Assert.That(
                Vector3.Distance(camera.transform.position, startCameraPosition),
                Is.GreaterThan(0.1f));
        }
    }
}
