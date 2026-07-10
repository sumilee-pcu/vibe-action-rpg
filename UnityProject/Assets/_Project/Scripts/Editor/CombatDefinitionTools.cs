using System;
using TinyVanguard.Combat;
using TinyVanguard.Enemies;
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
        public const string MeleeGruntActorPath =
            "Assets/_Project/Data/Actors/MeleeGrunt.asset";
        public const string MeleeGruntAttackPath =
            "Assets/_Project/Data/Attacks/MeleeGruntAttack.asset";
        public const string MeleeGruntDefinitionPath =
            "Assets/_Project/Data/Enemies/MeleeGrunt.asset";

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
            EnsureDataFolders();
            var player = LoadOrCreate<ActorDefinition>(PlayerDefinitionPath);
            ConfigureActor(player);

            var basicAttack = LoadOrCreate<AttackDefinition>(BasicAttackDefinitionPath);
            ConfigureAttack(basicAttack);

            var meleeGruntActor = LoadOrCreate<ActorDefinition>(MeleeGruntActorPath);
            ConfigureMeleeGruntActor(meleeGruntActor);

            var meleeGruntAttack = LoadOrCreate<AttackDefinition>(MeleeGruntAttackPath);
            ConfigureMeleeGruntAttack(meleeGruntAttack);

            var meleeGrunt = LoadOrCreate<EnemyDefinition>(MeleeGruntDefinitionPath);
            ConfigureMeleeGrunt(meleeGrunt, meleeGruntActor, meleeGruntAttack);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Validate(player, basicAttack, meleeGrunt);
            Debug.Log("[Tiny Vanguard] Default combat definitions created and validated.");
        }

        private static void EnsureDataFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Data/Enemies"))
            {
                AssetDatabase.CreateFolder("Assets/_Project/Data", "Enemies");
            }
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

        private static void ConfigureMeleeGruntActor(ActorDefinition actor)
        {
            var serialized = new SerializedObject(actor);
            Set(serialized, "_identifier", "melee-grunt");
            Set(serialized, "_displayName", "Melee Grunt");
            Set(serialized, "_level", 1);
            Set(serialized, "_maximumHealth", 60);
            Set(serialized, "_attackPower", 8);
            Set(serialized, "_moveSpeed", 3.5f);
            Set(serialized, "_experienceReward", 25);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(actor);
        }

        private static void ConfigureMeleeGruntAttack(AttackDefinition attack)
        {
            var serialized = new SerializedObject(attack);
            Set(serialized, "_identifier", "melee-grunt-swipe");
            Set(serialized, "_displayName", "Grunt Swipe");
            Set(serialized, "_baseDamage", 8);
            Set(serialized, "_range", 1.5f);
            Set(serialized, "_hitRadius", 0.45f);
            Set(serialized, "_activeStartTime", 0.35f);
            Set(serialized, "_activeEndTime", 0.5f);
            Set(serialized, "_cooldown", 1.2f);
            Set(serialized, "_staggerDuration", 0.15f);
            Set(serialized, "_movementMultiplier", 0f);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(attack);
        }

        private static void ConfigureMeleeGrunt(
            EnemyDefinition enemy,
            ActorDefinition actor,
            AttackDefinition attack)
        {
            var serialized = new SerializedObject(enemy);
            Set(serialized, "_actor", actor);
            Set(serialized, "_attack", attack);
            Set(serialized, "_detectionRange", 7f);
            Set(serialized, "_attackRange", 1.5f);
            Set(serialized, "_disengageRange", 12f);
            Set(serialized, "_homeTolerance", 0.25f);
            Set(serialized, "_attackCooldown", 1.2f);
            Set(serialized, "_navigationStoppingDistance", 1.25f);
            Set(serialized, "_separationRadius", 0.75f);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(enemy);
        }

        private static void Validate(
            ActorDefinition actor,
            AttackDefinition attack,
            EnemyDefinition meleeGrunt)
        {
            Require(actor.Identifier == "player", "Player identifier is invalid.");
            Require(actor.MaximumHealth == 100, "Player maximum health is invalid.");
            Require(attack.Identifier == "basic-attack", "Attack identifier is invalid.");
            Require(attack.ActiveStartTime < attack.ActiveEndTime,
                "Attack active window is invalid.");
            Require(attack.Cooldown >= attack.ActiveEndTime,
                "Attack cooldown must cover the active window.");
            Require(meleeGrunt.Actor != null && meleeGrunt.Attack != null,
                "Melee Grunt composition is invalid.");
            Require(meleeGrunt.AttackRange <= meleeGrunt.DetectionRange,
                "Melee Grunt attack range exceeds detection range.");
            Require(meleeGrunt.DetectionRange <= meleeGrunt.DisengageRange,
                "Melee Grunt detection range exceeds disengage range.");
            Require(meleeGrunt.ExperienceReward == 25,
                "Melee Grunt experience reward is invalid.");
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

        private static void Set(
            SerializedObject target,
            string name,
            UnityEngine.Object value)
        {
            var property = RequireProperty(target, name);
            property.objectReferenceValue = value;
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
