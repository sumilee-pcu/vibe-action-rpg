using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

namespace TinyVanguard.Editor
{
    public static class ProjectFoundationTools
    {
        private const string CombatSandboxScene = "Assets/_Project/Scenes/CombatSandbox.unity";
        private const string VerticalSliceScene = "Assets/_Project/Scenes/VerticalSlice.unity";

        [MenuItem("Tiny Vanguard/Validate Project Foundation")]
        public static void ValidateProjectFoundation()
        {
            Require(Application.unityVersion == "6000.3.11f1",
                $"Expected Unity 6000.3.11f1 but found {Application.unityVersion}.");
            Require(EditorSettings.serializationMode == SerializationMode.ForceText,
                "Asset Serialization must be Force Text.");
            Require(GraphicsSettings.defaultRenderPipeline != null,
                "A URP render pipeline asset must be assigned.");
            Require(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Packages/manifest.json")),
                "Packages/manifest.json is missing.");
            Require(AssetDatabase.LoadAssetAtPath<SceneAsset>(CombatSandboxScene) != null,
                $"Missing scene: {CombatSandboxScene}");
            Require(AssetDatabase.LoadAssetAtPath<SceneAsset>(VerticalSliceScene) != null,
                $"Missing scene: {VerticalSliceScene}");

            var enabledScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            Require(enabledScenes.SequenceEqual(new[] { CombatSandboxScene, VerticalSliceScene }),
                "Build Settings must contain CombatSandbox followed by VerticalSlice.");

            Debug.Log("[Tiny Vanguard] Project foundation validation passed.");
        }

        [MenuItem("Tiny Vanguard/Build/Build macOS Apple Silicon")]
        public static void BuildMacOSAppleSilicon()
        {
            ValidateProjectFoundation();
            PlayerSettings.SetArchitecture(
                NamedBuildTarget.Standalone,
                (int)OSArchitecture.ARM64);
            SetMacOSBuildArchitecture(OSArchitecture.ARM64);

            var buildPath = Path.GetFullPath(
                Path.Combine(Application.dataPath, "../../Builds/macOS/TinyVanguard-Arm64.app"));
            Directory.CreateDirectory(Path.GetDirectoryName(buildPath)
                ?? throw new InvalidOperationException("Unable to resolve the macOS build directory."));

            var options = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => scene.path)
                    .ToArray(),
                locationPathName = buildPath,
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"macOS build failed with result {report.summary.result} and " +
                    $"{report.summary.totalErrors} error(s).");
            }

            Debug.Log($"[Tiny Vanguard] macOS build succeeded: {buildPath}");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new BuildFailedException(message);
            }
        }

        private static void SetMacOSBuildArchitecture(OSArchitecture architecture)
        {
            const string typeName =
                "UnityEditor.OSXStandalone.UserBuildSettings, UnityEditor.OSXStandalone.Extensions";
            var settingsType = Type.GetType(typeName);
            var architectureProperty = settingsType?.GetProperty(
                "architecture",
                BindingFlags.Public | BindingFlags.Static);

            Require(architectureProperty != null,
                "Unable to locate the macOS build architecture setting.");
            architectureProperty!.SetValue(null, architecture);
        }
    }
}
