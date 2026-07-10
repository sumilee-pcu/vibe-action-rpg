using System.Collections;
using System.Linq;
using NUnit.Framework;
using TinyVanguard.CameraControl;
using TinyVanguard.Player;
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
            var occlusionCases = GameObject.Find("Camera Occlusion Cases");
            var cinemachineCamera = Object.FindFirstObjectByType<CinemachineCamera>();
            var orbitalFollow = Object.FindFirstObjectByType<CinemachineOrbitalFollow>();
            var movementController = player!.GetComponent<PlayerMovementController>();
            Assert.That(player, Is.Not.Null);
            Assert.That(camera, Is.Not.Null);
            Assert.That(occlusionCases, Is.Not.Null);
            Assert.That(cinemachineCamera, Is.Not.Null);
            Assert.That(orbitalFollow, Is.Not.Null);
            Assert.That(movementController, Is.Not.Null);

            occlusionCases!.SetActive(false);
            movementController!.enabled = false;
            orbitalFollow!.TrackerSettings.PositionDamping = Vector3.zero;
            cinemachineCamera!.PreviousStateIsValid = false;
            yield return null;

            var startCameraPosition = camera!.transform.position;
            player!.transform.position += Vector3.right * 3f;
            cinemachineCamera.PreviousStateIsValid = false;
            yield return null;
            yield return null;

            Assert.That(
                Vector3.Distance(camera.transform.position, startCameraPosition),
                Is.GreaterThan(2.5f));
        }

        [UnityTest]
        public IEnumerator DirectWallPullsCameraForwardAndClearsLineOfSight()
        {
            yield return LoadCombatSandbox();

            var camera = Camera.main;
            var player = GameObject.FindWithTag("Player");
            var cinemachineCamera = Object.FindFirstObjectByType<CinemachineCamera>();
            var orbitalFollow = Object.FindFirstObjectByType<CinemachineOrbitalFollow>();
            var deoccluder = Object.FindFirstObjectByType<CinemachineDeoccluder>();
            var wall = GameObject.Find("Direct Occlusion Wall");

            Assert.That(camera, Is.Not.Null);
            Assert.That(player, Is.Not.Null);
            var cameraTarget = player!.transform.Find("Camera Target");
            Assert.That(cameraTarget, Is.Not.Null);
            Assert.That(cinemachineCamera, Is.Not.Null);
            Assert.That(orbitalFollow, Is.Not.Null);
            Assert.That(deoccluder, Is.Not.Null);
            Assert.That(wall, Is.Not.Null);

            yield return new WaitForSeconds(0.25f);

            var targetToCamera = camera!.transform.position - cameraTarget!.position;
            var wallCollider = wall!.GetComponent<BoxCollider>();
            Assert.That(deoccluder!.CameraWasDisplaced(cinemachineCamera!), Is.True);
            Assert.That(targetToCamera.magnitude,
                Is.LessThan(orbitalFollow!.Radius - 1f));
            Assert.That(camera.transform.position.z,
                Is.GreaterThan(wallCollider.bounds.max.z));

            var blockingWallHits = Physics.RaycastAll(
                    cameraTarget.position,
                    targetToCamera.normalized,
                    targetToCamera.magnitude,
                    1 << 0,
                    QueryTriggerInteraction.Ignore)
                .Where(hit => hit.collider == wallCollider);
            Assert.That(blockingWallHits, Is.Empty);
        }

        [UnityTest]
        public IEnumerator SidePillarIsHandledAtQuarterOrbit()
        {
            yield return LoadCombatSandbox();

            var directWall = GameObject.Find("Direct Occlusion Wall");
            var diagonalBlock = GameObject.Find("Diagonal Occlusion Block");
            var camera = Camera.main;
            var player = GameObject.FindWithTag("Player");
            var cinemachineCamera = Object.FindFirstObjectByType<CinemachineCamera>();
            var orbitalFollow = Object.FindFirstObjectByType<CinemachineOrbitalFollow>();
            var deoccluder = Object.FindFirstObjectByType<CinemachineDeoccluder>();
            var pillar = GameObject.Find("Side Occlusion Pillar");

            Assert.That(directWall, Is.Not.Null);
            Assert.That(diagonalBlock, Is.Not.Null);
            Assert.That(camera, Is.Not.Null);
            Assert.That(player, Is.Not.Null);
            Assert.That(cinemachineCamera, Is.Not.Null);
            Assert.That(orbitalFollow, Is.Not.Null);
            Assert.That(deoccluder, Is.Not.Null);
            Assert.That(pillar, Is.Not.Null);
            var cameraTarget = player!.transform.Find("Camera Target");
            Assert.That(cameraTarget, Is.Not.Null);

            directWall!.SetActive(false);
            diagonalBlock!.SetActive(false);
            orbitalFollow!.HorizontalAxis.Value = 90f;
            orbitalFollow.VerticalAxis.Value = 20f;
            yield return new WaitForSeconds(0.75f);

            var pillarCollider = pillar!.GetComponent<BoxCollider>();
            var targetDistance = Vector3.Distance(
                cameraTarget!.position,
                camera!.transform.position);

            Assert.That(deoccluder!.CameraWasDisplaced(cinemachineCamera!), Is.True);
            Assert.That(targetDistance, Is.LessThan(orbitalFollow.Radius - 0.5f));
            Assert.That(camera.transform.position.x,
                Is.GreaterThan(pillarCollider.bounds.max.x));
        }

        [UnityTest]
        public IEnumerator CameraReturnsTowardOrbitRadiusAfterObstaclesAreCleared()
        {
            yield return LoadCombatSandbox();

            var camera = Camera.main;
            var player = GameObject.FindWithTag("Player");
            var occlusionCases = GameObject.Find("Camera Occlusion Cases");
            var cinemachineCamera = Object.FindFirstObjectByType<CinemachineCamera>();
            var deoccluder = Object.FindFirstObjectByType<CinemachineDeoccluder>();

            Assert.That(camera, Is.Not.Null);
            Assert.That(player, Is.Not.Null);
            Assert.That(occlusionCases, Is.Not.Null);
            Assert.That(cinemachineCamera, Is.Not.Null);
            Assert.That(deoccluder, Is.Not.Null);
            var cameraTarget = player!.transform.Find("Camera Target");
            Assert.That(cameraTarget, Is.Not.Null);

            yield return new WaitForSeconds(0.25f);
            var obstructedDistance = Vector3.Distance(
                cameraTarget!.position,
                camera!.transform.position);
            var obstructedCorrection = deoccluder!.GetCameraDisplacementDistance(
                cinemachineCamera!);

            occlusionCases!.SetActive(false);
            yield return new WaitForSeconds(1.2f);
            var clearedDistance = Vector3.Distance(
                cameraTarget.position,
                camera.transform.position);
            var clearedCorrection = deoccluder.GetCameraDisplacementDistance(
                cinemachineCamera);

            Assert.That(clearedDistance, Is.GreaterThan(obstructedDistance + 1f));
            Assert.That(clearedCorrection, Is.LessThan(obstructedCorrection - 1f));
        }

        private static IEnumerator LoadCombatSandbox()
        {
            yield return SceneManager.LoadSceneAsync(
                "CombatSandbox",
                LoadSceneMode.Single);
            yield return null;
            yield return null;
        }
    }
}
