using UnityEngine;

namespace TinyVanguard.Combat
{
    [CreateAssetMenu(
        fileName = "ActorDefinition",
        menuName = "Tiny Vanguard/Combat/Actor Definition")]
    public sealed class ActorDefinition : ScriptableObject
    {
        [SerializeField] private string _identifier = "actor";
        [SerializeField] private string _displayName = "Actor";
        [SerializeField, Min(1)] private int _level = 1;
        [SerializeField, Min(1)] private int _maximumHealth = 100;
        [SerializeField, Min(0)] private int _attackPower = 10;
        [SerializeField, Min(0f)] private float _moveSpeed = 5f;
        [SerializeField, Min(0)] private int _experienceReward;

        public string Identifier => _identifier;
        public string DisplayName => _displayName;
        public int Level => _level;
        public int MaximumHealth => _maximumHealth;
        public int AttackPower => _attackPower;
        public float MoveSpeed => _moveSpeed;
        public int ExperienceReward => _experienceReward;

        private void OnValidate()
        {
            _identifier = string.IsNullOrWhiteSpace(_identifier)
                ? name
                : _identifier.Trim();
            _displayName = string.IsNullOrWhiteSpace(_displayName)
                ? name
                : _displayName.Trim();
            _level = Mathf.Max(1, _level);
            _maximumHealth = Mathf.Max(1, _maximumHealth);
            _attackPower = Mathf.Max(0, _attackPower);
            _moveSpeed = Mathf.Max(0f, _moveSpeed);
            _experienceReward = Mathf.Max(0, _experienceReward);
        }
    }
}
