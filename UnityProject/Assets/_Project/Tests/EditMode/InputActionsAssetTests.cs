using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.InputSystem;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class InputActionsAssetTests
    {
        private const string AssetPath =
            "Assets/_Project/Input/TinyVanguardInput.inputactions";
        private const string ProjectWideActionsKey = "com.unity.input.settings.actions";

        private InputActionAsset _asset = null!;

        [SetUp]
        public void SetUp()
        {
            _asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
            Assert.That(_asset, Is.Not.Null, $"Missing input actions asset at {AssetPath}.");
        }

        [Test]
        public void AssetSeparatesGameplaySystemAndUIMaps()
        {
            var mapNames = _asset.actionMaps.Select(map => map.name);

            Assert.That(mapNames, Is.EquivalentTo(new[] { "Gameplay", "System", "UI" }));
        }

        [Test]
        public void GameplayMapDefinesRequiredActions()
        {
            var gameplay = _asset.FindActionMap("Gameplay", true);

            AssertAction(gameplay, "Move", InputActionType.Value, "Vector2");
            AssertAction(gameplay, "Look", InputActionType.Value, "Vector2");
            AssertAction(gameplay, "Attack", InputActionType.Button, "Button");
            AssertAction(gameplay, "Dodge", InputActionType.Button, "Button");
            AssertAction(gameplay, "Skill1", InputActionType.Button, "Button");
            AssertAction(gameplay, "Skill2", InputActionType.Button, "Button");

            AssertBinding(gameplay, "Move", "<Keyboard>/w");
            AssertBinding(gameplay, "Move", "<Keyboard>/a");
            AssertBinding(gameplay, "Move", "<Keyboard>/s");
            AssertBinding(gameplay, "Move", "<Keyboard>/d");
            AssertBinding(gameplay, "Look", "<Pointer>/delta");
            AssertBinding(gameplay, "Attack", "<Mouse>/leftButton");
            AssertBinding(gameplay, "Dodge", "<Keyboard>/space");
            AssertBinding(gameplay, "Skill1", "<Keyboard>/q");
            AssertBinding(gameplay, "Skill2", "<Keyboard>/e");
        }

        [Test]
        public void SystemMapKeepsPauseSeparateFromGameplay()
        {
            var system = _asset.FindActionMap("System", true);

            AssertAction(system, "Pause", InputActionType.Button, "Button");
            AssertBinding(system, "Pause", "<Keyboard>/escape");
        }

        [Test]
        public void UIMapDefinesKeyboardAndMouseNavigation()
        {
            var ui = _asset.FindActionMap("UI", true);

            AssertAction(ui, "Navigate", InputActionType.PassThrough, "Vector2");
            AssertAction(ui, "Submit", InputActionType.Button, "Button");
            AssertAction(ui, "Cancel", InputActionType.Button, "Button");
            AssertAction(ui, "Point", InputActionType.PassThrough, "Vector2");
            AssertAction(ui, "Click", InputActionType.PassThrough, "Button");
            AssertAction(ui, "ScrollWheel", InputActionType.PassThrough, "Vector2");

            AssertBinding(ui, "Navigate", "<Keyboard>/upArrow");
            AssertBinding(ui, "Submit", "<Keyboard>/enter");
            AssertBinding(ui, "Cancel", "<Keyboard>/backspace");
            AssertBinding(ui, "Point", "<Mouse>/position");
            AssertBinding(ui, "Click", "<Mouse>/leftButton");
            AssertBinding(ui, "ScrollWheel", "<Mouse>/scroll");
        }

        [Test]
        public void AssetUsesKeyboardAndMouseMvpControlScheme()
        {
            Assert.That(
                _asset.controlSchemes.Select(scheme => scheme.name),
                Is.EquivalentTo(new[] { "Keyboard&Mouse" }));
        }

        [Test]
        public void AssetRemainsTheProjectWideDefault()
        {
            var hasConfiguredAsset = EditorBuildSettings.TryGetConfigObject(
                ProjectWideActionsKey,
                out InputActionAsset configuredAsset);

            Assert.That(hasConfiguredAsset, Is.True);
            Assert.That(configuredAsset, Is.SameAs(_asset));
        }

        private static void AssertAction(
            InputActionMap map,
            string actionName,
            InputActionType expectedType,
            string expectedControlType)
        {
            var action = map.FindAction(actionName, true);

            Assert.That(action.type, Is.EqualTo(expectedType), actionName);
            Assert.That(action.expectedControlType, Is.EqualTo(expectedControlType), actionName);
        }

        private static void AssertBinding(
            InputActionMap map,
            string actionName,
            string expectedPath)
        {
            var action = map.FindAction(actionName, true);
            var hasBinding = action.bindings.Any(binding => binding.path == expectedPath);

            Assert.That(
                hasBinding,
                Is.True,
                $"{map.name}/{actionName} is missing binding {expectedPath}.");
        }
    }
}
