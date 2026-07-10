using UnityEngine;

namespace TinyVanguard.Combat
{
    [CreateAssetMenu(
        fileName = "AttackDefinition",
        menuName = "Tiny Vanguard/Combat/Attack Definition")]
    public sealed class AttackDefinition : ScriptableObject
    {
        [SerializeField] private string _identifier = "attack";
        [SerializeField] private string _displayName = "Attack";
        [SerializeField, Min(0)] private int _baseDamage = 10;
        [SerializeField, Min(0f)] private float _range = 1.5f;
        [SerializeField, Min(0f)] private float _hitRadius = 0.6f;
        [SerializeField, Min(0f)] private float _activeStartTime = 0.15f;
        [SerializeField, Min(0f)] private float _activeEndTime = 0.3f;
        [SerializeField, Min(0f)] private float _cooldown = 0.5f;
        [SerializeField, Min(0f)] private float _staggerDuration = 0.1f;
        [SerializeField, Range(0f, 1f)] private float _movementMultiplier = 0.25f;

        public string Identifier => _identifier;
        public string DisplayName => _displayName;
        public int BaseDamage => _baseDamage;
        public float Range => _range;
        public float HitRadius => _hitRadius;
        public float ActiveStartTime => _activeStartTime;
        public float ActiveEndTime => _activeEndTime;
        public float Cooldown => _cooldown;
        public float StaggerDuration => _staggerDuration;
        public float MovementMultiplier => _movementMultiplier;

        private void OnValidate()
        {
            _identifier = string.IsNullOrWhiteSpace(_identifier)
                ? name
                : _identifier.Trim();
            _displayName = string.IsNullOrWhiteSpace(_displayName)
                ? name
                : _displayName.Trim();
            _baseDamage = Mathf.Max(0, _baseDamage);
            _range = Mathf.Max(0f, _range);
            _hitRadius = Mathf.Max(0f, _hitRadius);
            _activeStartTime = Mathf.Max(0f, _activeStartTime);
            _activeEndTime = Mathf.Max(_activeStartTime, _activeEndTime);
            _cooldown = Mathf.Max(_activeEndTime, _cooldown);
            _staggerDuration = Mathf.Max(0f, _staggerDuration);
            _movementMultiplier = Mathf.Clamp01(_movementMultiplier);
        }
    }
}
