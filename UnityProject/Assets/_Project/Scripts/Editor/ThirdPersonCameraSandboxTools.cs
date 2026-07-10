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
        private const string OcclusionCasesName = "Camera Occlusion Cases";

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

            var deoccluder = cameraRig.AddComponent<CinemachineDeoccluder>();
            ConfigureDeoccluder(deoccluder);

            var controller = cameraRig.AddComponent<ThirdPersonCameraController>();
            controller.Configure(
                inputActions,
                orbitalFollow,
                Sensitivity,
                VerticalLimits);

            camera.transform.position = new Vector3(0f, 3.45f, -5.64f);
            camera.transform.LookAt(cameraTarget.position);
            var occlusionCases = CreateOcclusionCases(scene);

            ValidateCamera(
                inputActions,
                camera,
                cameraTarget,
                brain,
                cinemachineCamera,
                orbitalFollow,
                rotationComposer,
                deoccluder,
                controller,
                occlusionCases);

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

        private static void ConfigureDeoccluder(CinemachineDeoccluder deoccluder)
        {
            deoccluder.CollideAgainst = 1 << 0;
            deoccluder.IgnoreTag = "Player";
            deoccluder.TransparentLayers = 0;
            deoccluder.MinimumDistanceFromTarget = 0.3f;
            deoccluder.AvoidObstacles = new CinemachineDeoccluder.ObstacleAvoidance
            {
                Enabled = true,
                DistanceLimit = 0f,
                MinimumOcclusionTime = 0f,
                CameraRadius = 0.25f,
                Strategy = CinemachineDeoccluder.ObstacleAvoidance
                    .ResolutionStrategy.PullCameraForward,
                MaximumEffort = 4,
                SmoothingTime = 0f,
                Damping = 0.05f,
                DampingWhenOccluded = 0.05f
            };

            var shotQuality = deoccluder.ShotQualityEvaluation;
            shotQuality.Enabled = false;
            deoccluder.ShotQualityEvaluation = shotQuality;
        }

        private static GameObject CreateOcclusionCases(Scene scene)
        {
            DestroyRootIfPresent(scene, OcclusionCasesName);

            var root = new GameObject(OcclusionCasesName);
            CreateObstacle(
                root.transform,
                "Direct Occlusion Wall",
                new Vector3(0f, 1.75f, -3f),
                new Vector3(4f, 3.5f, 0.5f));
            CreateObstacle(
                root.transform,
                "Side Occlusion Pillar",
                new Vector3(-3f, 1.75f, 0f),
                new Vector3(0.75f, 3.5f, 0.75f));
            CreateObstacle(
                root.transform,
                "Diagonal Occlusion Block",
                new Vector3(2.25f, 1.75f, -2.25f),
                new Vector3(1f, 3.5f, 1f));

            return root;
        }

        private static void CreateObstacle(
            Transform parent,
            string name,
            Vector3 position,
            Vector3 scale)
        {
            var obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = name;
            obstacle.transform.SetParent(parent, true);
            obstacle.transform.position = position;
            obstacle.transform.localScale = scale;
            obstacle.isStatic = true;
        }

        private static void ValidateCamera(
            InputActionAsset inputActions,
            Camera camera,
            Transform cameraTarget,
            CinemachineBrain brain,
            CinemachineCamera cinemachineCamera,
            CinemachineOrbitalFollow orbitalFollow,
            CinemachineRotationComposer rotationComposer,
            CinemachineDeoccluder deoccluder,
            ThirdPersonCameraController controller,
            GameObject occlusionCases)
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
            Require(deoccluder.AvoidObstacles.Enabled,
                "Cinemachine Deoccluder obstacle avoidance must be enabled.");
            Require(deoccluder.CollideAgainst == (1 << 0),
                "Deoccluder must collide with the prototype world layer.");
            Require(deoccluder.IgnoreTag == "Player",
                "Deoccluder must ignore the Player tag.");
            Require(deoccluder.AvoidObstacles.CameraRadius > 0f,
                "Deoccluder camera radius must be positive.");
            Require(controller.InputActions == inputActions,
                "Camera controller must reference TinyVanguardInput.");
            Require(controller.OrbitalFollow == orbitalFollow,
                "Camera controller must reference Orbital Follow.");
            Require(camera.CompareTag("MainCamera"), "Output camera must be MainCamera.");
            RequireOcclusionCase(occlusionCases.transform, "Direct Occlusion Wall");
            RequireOcclusionCase(occlusionCases.transform, "Side Occlusion Pillar");
            RequireOcclusionCase(occlusionCases.transform, "Diagonal Occlusion Block");
        }

        private static void RequireOcclusionCase(Transform root, string childName)
        {
            var obstacle = root.Find(childName);
            Require(obstacle != null, $"Missing camera occlusion case: {childName}");
            Require(obstacle!.GetComponent<BoxCollider>() != null,
                $"Camera occlusion case requires a BoxCollider: {childName}");
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
