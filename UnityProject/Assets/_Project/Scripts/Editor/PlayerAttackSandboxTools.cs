using System;
using System.Linq;
using TinyVanguard.Combat;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TinyVanguard.Editor
{
    public static class PlayerAttackSandboxTools
    {
        public const string AnimationFolder = "Assets/_Project/Art/Animations";
        public const string BasicAttackClipPath = AnimationFolder + "/BasicAttack.anim";
        public const string AnimatorControllerPath = AnimationFolder + "/PlayerCombat.controller";
        private const string ScenePath = "Assets/_Project/Scenes/CombatSandbox.unity";
        private const string InputActionsPath = "Assets/_Project/Input/TinyVanguardInput.inputactions";

        [MenuItem("Tiny Vanguard/Setup Player Attack Sandbox")]
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
            EnsureAnimationFolder();
            AssetDatabase.ImportAsset(
                CombatDefinitionTools.BasicAttackDefinitionPath,
                ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(
                InputActionsPath,
                ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var definition = AssetDatabase.LoadAssetAtPath<AttackDefinition>(
                CombatDefinitionTools.BasicAttackDefinitionPath);
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            Require(definition != null, "Create default combat definitions first.");
            Require(inputActions != null, "TinyVanguardInput is missing.");

            var clip = CreateAttackClip(definition!);
            var controller = CreateAnimatorController(clip);
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var player = scene.GetRootGameObjects().FirstOrDefault(root => root.CompareTag("Player"));
            Require(player != null, "CombatSandbox Player is missing.");

            var animator = player!.GetComponent<Animator>();
            if (animator == null)
            {
                animator = player.AddComponent<Animator>();
            }
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;

            var attackController = player.GetComponent<PlayerAttackController>();
            if (attackController == null)
            {
                attackController = player.AddComponent<PlayerAttackController>();
            }
            attackController.Configure(inputActions!, definition!, animator);

            Validate(clip, controller, animator, attackController, definition!);
            EditorUtility.SetDirty(player);
            EditorUtility.SetDirty(animator);
            EditorUtility.SetDirty(attackController);
            EditorSceneManager.MarkSceneDirty(scene);
            Require(EditorSceneManager.SaveScene(scene), "Failed to save CombatSandbox.");
            AssetDatabase.SaveAssets();
            Debug.Log("[Tiny Vanguard] Player attack animation and active window configured.");
        }

        private static AnimationClip CreateAttackClip(AttackDefinition definition)
        {
            AssetDatabase.DeleteAsset(BasicAttackClipPath);
            var clip = new AnimationClip
            {
                name = "BasicAttack",
                frameRate = 60f,
                wrapMode = WrapMode.Once
            };

            var scaleX = AnimationCurve.EaseInOut(0f, 1f, definition.ActiveStartTime, 1.25f);
            scaleX.AddKey(definition.ActiveEndTime, 1.1f);
            scaleX.AddKey(definition.Cooldown, 1f);
            var scaleZ = AnimationCurve.EaseInOut(0f, 1f, definition.ActiveStartTime, 1.35f);
            scaleZ.AddKey(definition.ActiveEndTime, 1.15f);
            scaleZ.AddKey(definition.Cooldown, 1f);
            AnimationUtility.SetEditorCurve(
                clip,
                EditorCurveBinding.FloatCurve("Visual", typeof(Transform), "m_LocalScale.x"),
                scaleX);
            AnimationUtility.SetEditorCurve(
                clip,
                EditorCurveBinding.FloatCurve("Visual", typeof(Transform), "m_LocalScale.z"),
                scaleZ);
            AnimationUtility.SetAnimationEvents(clip, new[]
            {
                CreateEvent(definition.ActiveStartTime, nameof(PlayerAttackController.OpenHitWindow)),
                CreateEvent(definition.ActiveEndTime, nameof(PlayerAttackController.CloseHitWindow)),
                CreateEvent(definition.Cooldown, nameof(PlayerAttackController.CompleteAttack))
            });
            AssetDatabase.CreateAsset(clip, BasicAttackClipPath);
            return clip;
        }

        private static AnimatorController CreateAnimatorController(AnimationClip clip)
        {
            AssetDatabase.DeleteAsset(AnimatorControllerPath);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(
                AnimatorControllerPath);
            controller.AddParameter(PlayerAttackController.AttackTrigger, AnimatorControllerParameterType.Trigger);
            var stateMachine = controller.layers[0].stateMachine;
            var idle = stateMachine.AddState(PlayerAttackController.IdleState);
            var attack = stateMachine.AddState("BasicAttack");
            attack.motion = clip;
            stateMachine.defaultState = idle;

            var enterAttack = idle.AddTransition(attack);
            enterAttack.hasExitTime = false;
            enterAttack.duration = 0f;
            enterAttack.AddCondition(
                AnimatorConditionMode.If,
                0f,
                PlayerAttackController.AttackTrigger);

            var returnIdle = attack.AddTransition(idle);
            returnIdle.hasExitTime = true;
            returnIdle.exitTime = 1f;
            returnIdle.duration = 0f;
            return controller;
        }

        private static AnimationEvent CreateEvent(float time, string functionName)
        {
            return new AnimationEvent { time = time, functionName = functionName };
        }

        private static void Validate(
            AnimationClip clip,
            AnimatorController controller,
            Animator animator,
            PlayerAttackController attackController,
            AttackDefinition definition)
        {
            var events = AnimationUtility.GetAnimationEvents(clip);
            Require(events.Length == 3, "BasicAttack requires three animation events.");
            Require(Mathf.Approximately(events[0].time, definition.ActiveStartTime),
                "Hit window start event does not match AttackDefinition.");
            Require(Mathf.Approximately(events[1].time, definition.ActiveEndTime),
                "Hit window end event does not match AttackDefinition.");
            Require(Mathf.Approximately(events[2].time, definition.Cooldown),
                "Attack completion event does not match AttackDefinition.");
            Require(controller.parameters.Any(parameter =>
                    parameter.name == PlayerAttackController.AttackTrigger
                    && parameter.type == AnimatorControllerParameterType.Trigger),
                "Animator Attack trigger is missing.");
            Require(animator.runtimeAnimatorController == controller,
                "Player Animator controller is invalid.");
            Require(attackController.AttackDefinition == definition,
                "Player attack definition reference is invalid.");
        }

        private static void EnsureAnimationFolder()
        {
            if (!AssetDatabase.IsValidFolder(AnimationFolder))
            {
                AssetDatabase.CreateFolder("Assets/_Project/Art", "Animations");
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
