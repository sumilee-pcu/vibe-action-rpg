using System;
using System.Linq;
using TinyVanguard.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TinyVanguard.Editor
{
    public static class PlayerMovementSandboxTools
    {
        private const string ScenePath = "Assets/_Project/Scenes/CombatSandbox.unity";
        private const string InputActionsPath =
            "Assets/_Project/Input/TinyVanguardInput.inputactions";

        [MenuItem("Tiny Vanguard/Setup Player Movement Sandbox")]
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

        private static void ConfigureSandbox()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            Require(inputActions != null, $"Missing input actions asset: {InputActionsPath}");

            DestroyRootIfPresent(scene, "Player");
            DestroyRootIfPresent(scene, "Ground");

            var camera = Camera.main;
            Require(camera != null, "CombatSandbox requires a camera tagged MainCamera.");

            var ground = CreateGround();
            var player = CreatePlayer(inputActions!, camera!.transform);
            ConfigureCamera(camera, player.transform);
            ThirdPersonCameraSandboxTools.ConfigureLoadedScene(
                scene,
                inputActions,
                camera,
                player);

            ValidateScene(scene, inputActions, camera, player, ground);
            EditorSceneManager.MarkSceneDirty(scene);
            Require(EditorSceneManager.SaveScene(scene), $"Failed to save {ScenePath}.");

            Debug.Log("[Tiny Vanguard] Player movement sandbox configured and validated.");
        }

        private static GameObject CreateGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0f, -0.5f, 0f);
            ground.transform.localScale = new Vector3(20f, 1f, 20f);
            ground.isStatic = true;
            return ground;
        }

        private static GameObject CreatePlayer(
            InputActionAsset inputActions,
            Transform cameraTransform)
        {
            var player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = Vector3.zero;

            var characterController = player.AddComponent<CharacterController>();
            characterController.center = new Vector3(0f, 1f, 0f);
            characterController.height = 2f;
            characterController.radius = 0.5f;
            characterController.slopeLimit = 45f;
            characterController.stepOffset = 0.3f;
            characterController.skinWidth = 0.08f;
            characterController.minMoveDistance = 0f;

            var movementController = player.AddComponent<PlayerMovementController>();
            var serializedController = new SerializedObject(movementController);
            var cameraProperty = serializedController.FindProperty("_cameraTransform");
            var inputActionsProperty = serializedController.FindProperty("_inputActions");
            Require(cameraProperty != null, "Unable to serialize the movement camera reference.");
            Require(inputActionsProperty != null, "Unable to serialize the input actions reference.");
            cameraProperty!.objectReferenceValue = cameraTransform;
            inputActionsProperty!.objectReferenceValue = inputActions;
            serializedController.ApplyModifiedPropertiesWithoutUndo();

            CreateVisual(player.transform);
            return player;
        }

        private static void CreateVisual(Transform playerRoot)
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Visual";
            body.transform.SetParent(playerRoot, false);
            body.transform.localPosition = Vector3.up;
            UnityEngine.Object.DestroyImmediate(body.GetComponent<Collider>());

            var facingMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            facingMarker.name = "Facing Marker";
            facingMarker.transform.SetParent(playerRoot, false);
            facingMarker.transform.localPosition = new Vector3(0f, 1f, 0.55f);
            facingMarker.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            UnityEngine.Object.DestroyImmediate(facingMarker.GetComponent<Collider>());
        }

        private static void ConfigureCamera(Camera camera, Transform playerTransform)
        {
            camera.transform.position = new Vector3(-7f, 7f, -7f);
            camera.transform.LookAt(playerTransform.position + Vector3.up);
        }

        private static void ValidateScene(
            Scene scene,
            InputActionAsset inputActions,
            Camera camera,
            GameObject player,
            GameObject ground)
        {
            Require(scene.IsValid(), "CombatSandbox scene is invalid.");
            Require(player.GetComponent<CharacterController>() != null,
                "Player requires a CharacterController.");

            Require(inputActions.FindActionMap("Gameplay", true) != null,
                "TinyVanguardInput requires a Gameplay map.");

            var movementController = player.GetComponent<PlayerMovementController>();
            Require(movementController != null, "Player requires PlayerMovementController.");
            Require(movementController!.CameraTransform == camera.transform,
                "Player movement must reference Main Camera.");
            Require(movementController.InputActions == inputActions,
                "Player movement must reference TinyVanguardInput.");
            Require(player.transform.Find("Visual") != null, "Player visual is missing.");
            Require(player.transform.Find("Facing Marker") != null,
                "Player facing marker is missing.");
            Require(ground.GetComponent<BoxCollider>() != null,
                "Ground requires a BoxCollider.");
        }

        private static void DestroyRootIfPresent(Scene scene, string objectName)
        {
            var existing = scene
                .GetRootGameObjects()
                .FirstOrDefault(root => root.name == objectName);

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
