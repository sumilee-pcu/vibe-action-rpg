using System;
using System.Linq;
using TinyVanguard.CameraControl;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TinyVanguard.Editor
{
    public static class ThirdPersonCameraSandboxTools
    {
        private const string ScenePath = "Assets/_Project/Scenes/CombatSandbox.unity";
        private const string InputActionsPath =
            "Assets/_Project/Input/TinyVanguardInput.inputactions";
        private const string CameraRigName = "Third Person Camera";

        private static readonly Vector2 Sensitivity = new(0.12f, 0.08f);
        private static readonly Vector2 VerticalLimits = new(-20f, 65f);

        [MenuItem("Tiny Vanguard/Setup Third Person Camera Sandbox")]
        public static void ConfigureFromMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            ConfigureSandbox();
        }

        public static void ConfigureForBatch()
        {
            ConfigureSandbox();
        }

        public static void ConfigureLoadedScene(
            Scene scene,
            InputActionAsset inputActions,
            Camera camera,
            GameObject player)
        {
            Require(scene.IsValid(), "CombatSandbox scene is invalid.");
            Require(inputActions != null, "TinyVanguardInput is required.");
            Require(camera != null, "A Main Camera is required.");
            Require(player != null, "A Player root is required.");

            var cameraTarget = EnsureCameraTarget(player.transform);
            var brain = camera.GetComponent<CinemachineBrain>()
                ?? camera.gameObject.AddComponent<CinemachineBrain>();

            DestroyRootIfPresent(scene, CameraRigName);
            var cameraRig = new GameObject(CameraRigName);
            var cinemachineCamera = cameraRig.AddComponent<CinemachineCamera>();
            cinemachineCamera.Follow = cameraTarget;
            cinemachineCamera.LookAt = cameraTarget;

            var orbitalFollow = cameraRig.AddComponent<CinemachineOrbitalFollow>();
            orbitalFollow.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
            orbitalFollow.Radius = 6f;
            orbitalFollow.TargetOffset = Vector3.zero;

            var horizontalAxis = orbitalFollow.HorizontalAxis;
            horizontalAxis.Value = 0f;
            horizontalAxis.Center = 0f;
            horizontalAxis.Range = new Vector2(-180f, 180f);
            horizontalAxis.Wrap = true;
            horizontalAxis.Recentering.Enabled = false;
            orbitalFollow.HorizontalAxis = horizontalAxis;

            var verticalAxis = orbitalFollow.VerticalAxis;
            verticalAxis.Value = 20f;
            verticalAxis.Center = 20f;
            verticalAxis.Range = VerticalLimits;
            verticalAxis.Wrap = false;
            verticalAxis.Recentering.Enabled = false;
            orbitalFollow.VerticalAxis = verticalAxis;

            var rotationComposer = cameraRig.AddComponent<CinemachineRotationComposer>();
            rotationComposer.Damping = new Vector2(0.15f, 0.15f);
            rotationComposer.CenterOnActivate = true;

            var controller = cameraRig.AddComponent<ThirdPersonCameraController>();
            controller.Configure(
                inputActions,
                orbitalFollow,
                Sensitivity,
                VerticalLimits,
                true);

            camera.transform.position = new Vector3(0f, 3.45f, -5.64f);
            camera.transform.LookAt(cameraTarget.position);

            ValidateCamera(
                inputActions,
                camera,
                cameraTarget,
                brain,
                cinemachineCamera,
                orbitalFollow,
                rotationComposer,
                controller);

            EditorUtility.SetDirty(camera.gameObject);
            EditorUtility.SetDirty(cameraRig);
            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static void ConfigureSandbox()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            Require(inputActions != null, $"Missing input actions asset: {InputActionsPath}");

            var camera = Camera.main;
            var player = FindRoot(scene, "Player");
            Require(camera != null, "CombatSandbox requires a camera tagged MainCamera.");
            Require(player != null, "Run Setup Player Movement Sandbox first.");

            ConfigureLoadedScene(scene, inputActions!, camera!, player!);
            Require(EditorSceneManager.SaveScene(scene), $"Failed to save {ScenePath}.");
            Debug.Log("[Tiny Vanguard] Third-person camera configured and validated.");
        }

        private static Transform EnsureCameraTarget(Transform player)
        {
            var cameraTarget = player.Find("Camera Target");
            if (cameraTarget == null)
            {
                var targetObject = new GameObject("Camera Target");
                cameraTarget = targetObject.transform;
                cameraTarget.SetParent(player, false);
            }

            cameraTarget.localPosition = new Vector3(0f, 1.4f, 0f);
            cameraTarget.localRotation = Quaternion.identity;
            cameraTarget.localScale = Vector3.one;
            return cameraTarget;
        }

        private static void ValidateCamera(
            InputActionAsset inputActions,
            Camera camera,
            Transform cameraTarget,
            CinemachineBrain brain,
            CinemachineCamera cinemachineCamera,
            CinemachineOrbitalFollow orbitalFollow,
            CinemachineRotationComposer rotationComposer,
            ThirdPersonCameraController controller)
        {
            Require(brain != null, "Main Camera requires CinemachineBrain.");
            Require(cinemachineCamera.Follow == cameraTarget,
                "Cinemachine Camera must follow Camera Target.");
            Require(cinemachineCamera.LookAt == cameraTarget,
                "Cinemachine Camera must look at Camera Target.");
            Require(orbitalFollow.Radius > 0f, "Camera orbit radius must be positive.");
            Require(orbitalFollow.VerticalAxis.Range == VerticalLimits,
                "Camera vertical limits do not match the design values.");
            Require(!orbitalFollow.VerticalAxis.Wrap,
                "Camera vertical axis must clamp instead of wrap.");
            Require(rotationComposer.enabled, "Rotation Composer must be enabled.");
            Require(controller.InputActions == inputActions,
                "Camera controller must reference TinyVanguardInput.");
            Require(controller.OrbitalFollow == orbitalFollow,
                "Camera controller must reference Orbital Follow.");
            Require(camera.CompareTag("MainCamera"), "Output camera must be MainCamera.");
        }

        private static GameObject? FindRoot(Scene scene, string objectName)
        {
            return scene.GetRootGameObjects()
                .FirstOrDefault(root => root.name == objectName);
        }

        private static void DestroyRootIfPresent(Scene scene, string objectName)
        {
            var existing = FindRoot(scene, objectName);
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(existing);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
