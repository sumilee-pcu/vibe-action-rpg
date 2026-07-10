using TinyVanguard.Player;
using UnityEngine;

namespace TinyVanguard.Combat
{
    [DisallowMultipleComponent]
    public sealed class ActorHealth : MonoBehaviour
    {
        [SerializeField] private ActorDefinition _definition = null!;
        [SerializeField] private PlayerMovementController? _playerMovement;

        public ActorDefinition Definition => _definition;
        public HealthState State { get; private set; } = null!;

        public void Configure(
            ActorDefinition definition,
            PlayerMovementController? playerMovement = null)
        {
            _definition = definition;
            _playerMovement = playerMovement;
            Initialize();
        }

        public DamageResult ApplyDamage(int damage)
        {
            if (State == null)
            {
                Debug.LogError($"[{nameof(ActorHealth)}] Health state is not initialized.", this);
                return default;
            }

            return State.ApplyDamage(
                damage,
                _playerMovement != null && _playerMovement.IsInvulnerable);
        }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_definition == null)
            {
                return;
            }

            State = new HealthState(_definition.MaximumHealth);
        }
    }
}
