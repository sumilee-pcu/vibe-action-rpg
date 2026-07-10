using System;
using System.Linq;
using TinyVanguard.Session;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TinyVanguard.Editor
{
    public static class GameplayInputGateSandboxTools
    {
        private const string ScenePath = "Assets/_Project/Scenes/CombatSandbox.unity";
        private const string InputActionsPath =
            "Assets/_Project/Input/TinyVanguardInput.inputactions";
        private const string SessionRootName = "Game Session";

        [MenuItem("Tiny Vanguard/Setup Gameplay Input Gate")]
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

        public static GameplayInputGate ConfigureLoadedScene(
            Scene scene,
            InputActionAsset inputActions)
        {
            Require(scene.IsValid(), "CombatSandbox scene is invalid.");
            Require(inputActions != null, "TinyVanguardInput is required.");

            var sessionRoot = FindRoot(scene, SessionRootName)
                ?? new GameObject(SessionRootName);
            var inputGate = sessionRoot.GetComponent<GameplayInputGate>()
                ?? sessionRoot.AddComponent<GameplayInputGate>();
            inputGate.Configure(inputActions, GameSessionState.Playing, true);

            ValidateInputGate(inputGate, inputActions);
            EditorUtility.SetDirty(sessionRoot);
            EditorUtility.SetDirty(inputGate);
            EditorSceneManager.MarkSceneDirty(scene);
            return inputGate;
        }

        private static void ConfigureSandbox()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            Require(inputActions != null, $"Missing input actions asset: {InputActionsPath}");

            ConfigureLoadedScene(scene, inputActions!);
            Require(EditorSceneManager.SaveScene(scene), $"Failed to save {ScenePath}.");
            Debug.Log("[Tiny Vanguard] Gameplay input gate configured and validated.");
        }

        private static void ValidateInputGate(
            GameplayInputGate inputGate,
            InputActionAsset inputActions)
        {
            Require(inputGate.InputActions == inputActions,
                "Gameplay input gate must reference TinyVanguardInput.");
            Require(inputGate.InitialState == GameSessionState.Playing,
                "CombatSandbox must begin in Playing state.");
            Require(inputActions.FindActionMap("Gameplay", true) != null,
                "TinyVanguardInput requires a Gameplay map.");
            Require(inputActions.FindActionMap("System", true).FindAction("Pause", true) != null,
                "TinyVanguardInput requires System/Pause.");
            Require(inputActions.FindActionMap("UI", true) != null,
                "TinyVanguardInput requires a UI map.");
        }

        private static GameObject? FindRoot(Scene scene, string objectName)
        {
            return scene.GetRootGameObjects()
                .FirstOrDefault(root => root.name == objectName);
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
