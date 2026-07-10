using System;
using TinyVanguard.Combat;
using UnityEditor;
using UnityEngine;

namespace TinyVanguard.Editor
{
    public static class CombatDefinitionTools
    {
        public const string PlayerDefinitionPath =
            "Assets/_Project/Data/Actors/Player.asset";
        public const string BasicAttackDefinitionPath =
            "Assets/_Project/Data/Attacks/BasicAttack.asset";

        [MenuItem("Tiny Vanguard/Create Default Combat Definitions")]
        public static void CreateFromMenu()
        {
            CreateDefaults();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<ActorDefinition>(
                PlayerDefinitionPath);
        }

        public static void CreateForBatch()
        {
            CreateDefaults();
        }

        private static void CreateDefaults()
        {
            var player = LoadOrCreate<ActorDefinition>(PlayerDefinitionPath);
            ConfigureActor(player);

            var basicAttack = LoadOrCreate<AttackDefinition>(BasicAttackDefinitionPath);
            ConfigureAttack(basicAttack);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Validate(player, basicAttack);
            Debug.Log("[Tiny Vanguard] Default combat definitions created and validated.");
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void ConfigureActor(ActorDefinition actor)
        {
            var serialized = new SerializedObject(actor);
            Set(serialized, "_identifier", "player");
            Set(serialized, "_displayName", "Tiny Vanguard");
            Set(serialized, "_level", 1);
            Set(serialized, "_maximumHealth", 100);
            Set(serialized, "_attackPower", 10);
            Set(serialized, "_moveSpeed", 5f);
            Set(serialized, "_experienceReward", 0);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(actor);
        }

        private static void ConfigureAttack(AttackDefinition attack)
        {
            var serialized = new SerializedObject(attack);
            Set(serialized, "_identifier", "basic-attack");
            Set(serialized, "_displayName", "Basic Attack");
            Set(serialized, "_baseDamage", 15);
            Set(serialized, "_range", 1.5f);
            Set(serialized, "_hitRadius", 0.6f);
            Set(serialized, "_activeStartTime", 0.15f);
            Set(serialized, "_activeEndTime", 0.3f);
            Set(serialized, "_cooldown", 0.5f);
            Set(serialized, "_staggerDuration", 0.1f);
            Set(serialized, "_movementMultiplier", 0.25f);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(attack);
        }

        private static void Validate(
            ActorDefinition actor,
            AttackDefinition attack)
        {
            Require(actor.Identifier == "player", "Player identifier is invalid.");
            Require(actor.MaximumHealth == 100, "Player maximum health is invalid.");
            Require(attack.Identifier == "basic-attack", "Attack identifier is invalid.");
            Require(attack.ActiveStartTime < attack.ActiveEndTime,
                "Attack active window is invalid.");
            Require(attack.Cooldown >= attack.ActiveEndTime,
                "Attack cooldown must cover the active window.");
        }

        private static void Set(SerializedObject target, string name, string value)
        {
            var property = RequireProperty(target, name);
            property.stringValue = value;
        }

        private static void Set(SerializedObject target, string name, int value)
        {
            var property = RequireProperty(target, name);
            property.intValue = value;
        }

        private static void Set(SerializedObject target, string name, float value)
        {
            var property = RequireProperty(target, name);
            property.floatValue = value;
        }

        private static SerializedProperty RequireProperty(
            SerializedObject target,
            string name)
        {
            return target.FindProperty(name)
                ?? throw new InvalidOperationException($"Missing serialized property: {name}");
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
