using System;
using System.Linq;
using TinyVanguard.Combat;
using TinyVanguard.Enemies;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace TinyVanguard.Editor
{
    public static class EnemyNavigationSandboxTools
    {
        public const string NavigationFolder = "Assets/_Project/Navigation";
        public const string NavMeshDataPath = NavigationFolder + "/CombatSandboxNavMesh.asset";
        private const string ScenePath = "Assets/_Project/Scenes/CombatSandbox.unity";
        private const string EnemyName = "MeleeGrunt";

        [MenuItem("Tiny Vanguard/Setup Enemy Navigation Sandbox")]
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
            AssetDatabase.ImportAsset(
                CombatDefinitionTools.MeleeGruntDefinitionPath,
                ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            EnsureNavigationFolder();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var definition = AssetDatabase.LoadAssetAtPath<EnemyDefinition>(
                CombatDefinitionTools.MeleeGruntDefinitionPath);
            var playerDefinition = AssetDatabase.LoadAssetAtPath<ActorDefinition>(
                CombatDefinitionTools.PlayerDefinitionPath);
            Require(definition != null, "Create default enemy definitions first.");
            Require(playerDefinition != null, "Create default player definition first.");

            var surface = ConfigureSurface(scene);
            var playerHealth = ConfigurePlayerHealth(scene, playerDefinition!);
            var enemy = ConfigureEnemy(scene, definition!, playerHealth);
            BakeSurface(surface);
            Validate(surface, enemy, definition!);

            EditorSceneManager.MarkSceneDirty(scene);
            Require(EditorSceneManager.SaveScene(scene), "Failed to save CombatSandbox.");
            AssetDatabase.SaveAssets();
            Debug.Log("[Tiny Vanguard] Enemy NavMesh sandbox configured and validated.");
        }

        private static NavMeshSurface ConfigureSurface(Scene scene)
        {
            var navigation = scene.GetRootGameObjects()
                .FirstOrDefault(root => root.name == "Navigation");
            if (navigation == null)
            {
                navigation = new GameObject("Navigation");
            }

            var surface = navigation.GetComponent<NavMeshSurface>();
            if (surface == null)
            {
                surface = navigation.AddComponent<NavMeshSurface>();
            }

            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.layerMask = Physics.DefaultRaycastLayers;
            surface.ignoreNavMeshAgent = true;
            surface.ignoreNavMeshObstacle = true;
            EditorUtility.SetDirty(navigation);
            EditorUtility.SetDirty(surface);
            return surface;
        }

        private static EnemyNavigationController ConfigureEnemy(
            Scene scene,
            EnemyDefinition definition,
            ActorHealth playerHealth)
        {
            var enemy = scene.GetRootGameObjects()
                .FirstOrDefault(root => root.name == EnemyName);
            if (enemy == null)
            {
                enemy = new GameObject(EnemyName);
            }

            enemy.transform.position = new Vector3(4f, 0f, 4f);
            enemy.transform.rotation = Quaternion.identity;

            var collider = enemy.GetComponent<CapsuleCollider>();
            if (collider == null)
            {
                collider = enemy.AddComponent<CapsuleCollider>();
            }
            collider.center = Vector3.up;
            collider.height = 2f;
            collider.radius = 0.4f;

            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = enemy.AddComponent<NavMeshAgent>();
            }
            agent.height = 2f;
            agent.acceleration = 12f;
            agent.angularSpeed = 720f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.avoidancePriority = 50;
            agent.speed = definition.MoveSpeed;
            agent.stoppingDistance = definition.NavigationStoppingDistance;
            agent.radius = Mathf.Max(0.1f, definition.SeparationRadius * 0.5f);
            agent.autoBraking = true;

            var navigation = enemy.GetComponent<EnemyNavigationController>();
            if (navigation == null)
            {
                navigation = enemy.AddComponent<EnemyNavigationController>();
            }
            navigation.Configure(definition, agent);
            SerializeNavigationReferences(navigation, definition, agent);

            var health = enemy.GetComponent<ActorHealth>();
            if (health == null)
            {
                health = enemy.AddComponent<ActorHealth>();
            }
            health.Configure(definition.Actor);
            SerializeHealthDefinition(health, definition.Actor);

            var reactions = enemy.GetComponent<ActorReactionController>();
            if (reactions == null)
            {
                reactions = enemy.AddComponent<ActorReactionController>();
            }
            reactions.Configure(health);

            var brain = enemy.GetComponent<EnemyBrain>();
            if (brain == null)
            {
                brain = enemy.AddComponent<EnemyBrain>();
            }
            brain.Configure(
                definition,
                navigation,
                health,
                playerHealth.transform,
                playerHealth);
            SerializeBrainReferences(
                brain,
                definition,
                navigation,
                health,
                playerHealth);

            EnsureVisual(enemy.transform);
            EditorUtility.SetDirty(enemy);
            return navigation;
        }

        private static ActorHealth ConfigurePlayerHealth(
            Scene scene,
            ActorDefinition definition)
        {
            var player = scene.GetRootGameObjects()
                .FirstOrDefault(root => root.CompareTag("Player"));
            Require(player != null, "CombatSandbox Player is missing.");

            var health = player!.GetComponent<ActorHealth>();
            if (health == null)
            {
                health = player.AddComponent<ActorHealth>();
            }
            var movement = player.GetComponent<TinyVanguard.Player.PlayerMovementController>();
            health.Configure(definition, movement);

            var serialized = new SerializedObject(health);
            RequireProperty(serialized, "_definition").objectReferenceValue = definition;
            RequireProperty(serialized, "_playerMovement").objectReferenceValue = movement;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(health);
            return health;
        }

        private static void EnsureVisual(Transform enemy)
        {
            var visual = enemy.Find("Visual");
            if (visual != null)
            {
                return;
            }

            var visualObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visualObject.name = "Visual";
            visualObject.transform.SetParent(enemy, false);
            visualObject.transform.localPosition = Vector3.up;
            visualObject.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
            var generatedCollider = visualObject.GetComponent<Collider>();
            if (generatedCollider != null)
            {
                UnityEngine.Object.DestroyImmediate(generatedCollider);
            }
        }

        private static void SerializeNavigationReferences(
            EnemyNavigationController navigation,
            EnemyDefinition definition,
            NavMeshAgent agent)
        {
            var serialized = new SerializedObject(navigation);
            RequireProperty(serialized, "_definition").objectReferenceValue = definition;
            RequireProperty(serialized, "_agent").objectReferenceValue = agent;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(navigation);
        }

        private static void SerializeHealthDefinition(
            ActorHealth health,
            ActorDefinition definition)
        {
            var serialized = new SerializedObject(health);
            RequireProperty(serialized, "_definition").objectReferenceValue = definition;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(health);
        }

        private static void SerializeBrainReferences(
            EnemyBrain brain,
            EnemyDefinition definition,
            EnemyNavigationController navigation,
            ActorHealth selfHealth,
            ActorHealth playerHealth)
        {
            var serialized = new SerializedObject(brain);
            RequireProperty(serialized, "_definition").objectReferenceValue = definition;
            RequireProperty(serialized, "_navigation").objectReferenceValue = navigation;
            RequireProperty(serialized, "_selfHealth").objectReferenceValue = selfHealth;
            RequireProperty(serialized, "_target").objectReferenceValue = playerHealth.transform;
            RequireProperty(serialized, "_targetHealth").objectReferenceValue = playerHealth;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(brain);
        }

        private static void BakeSurface(NavMeshSurface surface)
        {
            surface.RemoveData();
            surface.navMeshData = null;
            AssetDatabase.DeleteAsset(NavMeshDataPath);
            surface.BuildNavMesh();
            Require(surface.navMeshData != null, "NavMesh bake produced no data.");
            AssetDatabase.CreateAsset(surface.navMeshData, NavMeshDataPath);
            EditorUtility.SetDirty(surface);
        }

        private static void Validate(
            NavMeshSurface surface,
            EnemyNavigationController enemy,
            EnemyDefinition definition)
        {
            Require(surface.navMeshData != null, "CombatSandbox NavMesh data is missing.");
            Require(
                AssetDatabase.GetAssetPath(surface.navMeshData) == NavMeshDataPath,
                "CombatSandbox NavMesh data path is invalid.");
            Require(enemy.Definition == definition, "Enemy definition reference is invalid.");
            Require(Mathf.Approximately(enemy.Agent.speed, definition.MoveSpeed),
                "Enemy NavMeshAgent speed is invalid.");
            Require(Mathf.Approximately(
                    enemy.Agent.stoppingDistance,
                    definition.NavigationStoppingDistance),
                "Enemy NavMeshAgent stopping distance is invalid.");
            var brain = enemy.GetComponent<EnemyBrain>();
            Require(brain != null && brain.Definition == definition,
                "Enemy brain definition reference is invalid.");
            Require(brain!.GetComponent<ActorHealth>() != null,
                "Enemy health adapter is missing.");
            Require(NavMesh.SamplePosition(
                    enemy.transform.position,
                    out var startHit,
                    1f,
                    NavMesh.AllAreas),
                "Enemy start position is outside the baked NavMesh.");
            Require(NavMesh.SamplePosition(
                    enemy.transform.position + Vector3.left * 2f,
                    out var destinationHit,
                    1f,
                    NavMesh.AllAreas),
                "Enemy validation destination is outside the baked NavMesh.");
            var path = new NavMeshPath();
            Require(NavMesh.CalculatePath(
                    startHit.position,
                    destinationHit.position,
                    NavMesh.AllAreas,
                    path),
                "Enemy validation path could not be calculated.");
            Require(path.status == NavMeshPathStatus.PathComplete,
                "Enemy validation path is incomplete.");
        }

        private static void EnsureNavigationFolder()
        {
            if (!AssetDatabase.IsValidFolder(NavigationFolder))
            {
                AssetDatabase.CreateFolder("Assets/_Project", "Navigation");
            }
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
