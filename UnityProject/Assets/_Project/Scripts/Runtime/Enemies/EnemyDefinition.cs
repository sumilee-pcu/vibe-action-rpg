using TinyVanguard.Combat;
using UnityEngine;

namespace TinyVanguard.Enemies
{
    [CreateAssetMenu(
        fileName = "EnemyDefinition",
        menuName = "Tiny Vanguard/Enemies/Enemy Definition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private ActorDefinition _actor = null!;
        [SerializeField] private AttackDefinition _attack = null!;
        [SerializeField, Min(0f)] private float _detectionRange = 7f;
        [SerializeField, Min(0f)] private float _attackRange = 1.5f;
        [SerializeField, Min(0f)] private float _disengageRange = 12f;
        [SerializeField, Min(0.01f)] private float _homeTolerance = 0.25f;
        [SerializeField, Min(0f)] private float _attackCooldown = 1.2f;
        [SerializeField, Min(0f)] private float _navigationStoppingDistance = 1.25f;
        [SerializeField, Min(0f)] private float _separationRadius = 0.75f;

        public ActorDefinition Actor => _actor;
        public AttackDefinition Attack => _attack;
        public float MoveSpeed => _actor != null ? _actor.MoveSpeed : 0f;
        public int ExperienceReward => _actor != null ? _actor.ExperienceReward : 0;
        public float DetectionRange => _detectionRange;
        public float AttackRange => _attackRange;
        public float DisengageRange => _disengageRange;
        public float HomeTolerance => _homeTolerance;
        public float AttackCooldown => _attackCooldown;
        public float NavigationStoppingDistance => _navigationStoppingDistance;
        public float SeparationRadius => _separationRadius;

        private void OnValidate()
        {
            _attackRange = Mathf.Max(0f, _attackRange);
            _detectionRange = Mathf.Max(_attackRange, _detectionRange);
            _disengageRange = Mathf.Max(_detectionRange, _disengageRange);
            _homeTolerance = Mathf.Max(0.01f, _homeTolerance);
            _attackCooldown = Mathf.Max(
                _attack != null ? _attack.Cooldown : 0f,
                _attackCooldown);
            _navigationStoppingDistance = Mathf.Clamp(
                _navigationStoppingDistance,
                0f,
                _attackRange);
            _separationRadius = Mathf.Max(0f, _separationRadius);
        }
    }
}
