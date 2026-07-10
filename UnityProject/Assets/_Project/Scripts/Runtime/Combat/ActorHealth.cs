using System;
using TinyVanguard.Player;
using UnityEngine;

namespace TinyVanguard.Combat
{
    [DisallowMultipleComponent]
    public sealed class ActorHealth : MonoBehaviour
    {
        [SerializeField] private ActorDefinition _definition = null!;
        [SerializeField] private PlayerMovementController? _playerMovement;

        public event Action<DamageAppliedEvent> DamageApplied = delegate { };
        public event Action<ActorHealth> Died = delegate { };

        public ActorDefinition Definition => _definition;
        public HealthState State { get; private set; } = null!;
        public bool CanAct => State != null && !State.IsDead;

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
            return ApplyDamage(damage, transform.position);
        }

        public DamageResult ApplyDamage(int damage, Vector3 worldPosition)
        {
            if (State == null)
            {
                Debug.LogError($"[{nameof(ActorHealth)}] Health state is not initialized.", this);
                return default;
            }

            var result = State.ApplyDamage(
                damage,
                _playerMovement != null && _playerMovement.IsInvulnerable);

            if (!result.WasApplied)
            {
                return result;
            }

            DamageApplied(new DamageAppliedEvent(this, result, worldPosition));
            if (result.CausedDeath)
            {
                Died(this);
            }

            return result;
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
