using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyVanguard.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class PlayerAttackController : MonoBehaviour
    {
        public const string AttackTrigger = "Attack";
        public const string IdleState = "Idle";
        private const int HitBufferCapacity = 16;
        private const float DefaultHitHeight = 1f;

        [SerializeField] private InputActionAsset _inputActions = null!;
        [SerializeField] private AttackDefinition _attackDefinition = null!;
        [SerializeField] private Animator _animator = null!;
        [SerializeField] private ActorHealth? _actorHealth;

        private InputActionMap _gameplayMap = null!;
        private InputAction _attackAction = null!;
        private readonly Collider[] _hitBuffer = new Collider[HitBufferCapacity];
        private double _attackStartedAt;

        public event Action<AttackTimingSample> AttackResolved = delegate { };

        public InputActionAsset InputActions => _inputActions;
        public AttackDefinition AttackDefinition => _attackDefinition;
        public Animator Animator => _animator;
        public bool IsAttackInProgress { get; private set; }
        public bool IsHitWindowActive { get; private set; }
        public int AttackSequence { get; private set; }
        public AttackExecution? CurrentExecution { get; private set; }
        public bool HasTimingSample { get; private set; }
        public AttackTimingSample LastTimingSample { get; private set; }

        public void Configure(
            InputActionAsset inputActions,
            AttackDefinition attackDefinition,
            Animator animator)
        {
            _inputActions = inputActions;
            _attackDefinition = attackDefinition;
            _animator = animator;
        }

        private void Awake()
        {
            if (_inputActions == null)
            {
                _inputActions = InputSystem.actions;
            }

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            if (_actorHealth == null)
            {
                _actorHealth = GetComponent<ActorHealth>();
            }

            _gameplayMap = _inputActions?.FindActionMap("Gameplay");
            _attackAction = _gameplayMap?.FindAction("Attack");
            if (_gameplayMap == null
                || _attackAction == null
                || _attackDefinition == null
                || _animator == null)
            {
                Debug.LogError(
                    $"[{nameof(PlayerAttackController)}] Input, attack definition and Animator are required.",
                    this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_attackAction != null)
            {
                _attackAction.performed += OnAttackPerformed;
            }

            if (_actorHealth != null)
            {
                _actorHealth.Died += OnActorDied;
            }
        }

        private void OnDisable()
        {
            if (_attackAction != null)
            {
                _attackAction.performed -= OnAttackPerformed;
            }

            if (_actorHealth != null)
            {
                _actorHealth.Died -= OnActorDied;
            }

            CancelAttack();
        }

        private void Update()
        {
            if (IsAttackInProgress && !_gameplayMap.enabled)
            {
                CancelAttack();
            }
        }

        public bool TryStartAttack()
        {
            if (!isActiveAndEnabled
                || !_gameplayMap.enabled
                || (_actorHealth != null && !_actorHealth.CanAct)
                || IsAttackInProgress)
            {
                return false;
            }

            IsAttackInProgress = true;
            IsHitWindowActive = false;
            AttackSequence++;
            CurrentExecution = new AttackExecution(AttackSequence);
            _attackStartedAt = Time.timeAsDouble;
            _animator.ResetTrigger(AttackTrigger);
            _animator.SetTrigger(AttackTrigger);
            return true;
        }

        public void OpenHitWindow()
        {
            if (!IsAttackInProgress)
            {
                return;
            }

            IsHitWindowActive = true;
            if (CurrentExecution == null)
            {
                return;
            }

            var hitCenter = transform.position
                + Vector3.up * DefaultHitHeight
                + transform.forward * _attackDefinition.Range;
            var colliderCount = Physics.OverlapSphereNonAlloc(
                hitCenter,
                _attackDefinition.HitRadius,
                _hitBuffer,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Collide);
            var appliedTargetCount = AttackHitResolver.ApplyDamage(
                _hitBuffer,
                colliderCount,
                CurrentExecution,
                _attackDefinition.BaseDamage,
                _actorHealth);
            var resolvedAt = Time.timeAsDouble;
            LastTimingSample = new AttackTimingSample(
                AttackSequence,
                _attackStartedAt,
                resolvedAt,
                appliedTargetCount);
            HasTimingSample = true;
            AttackResolved(LastTimingSample);
            Array.Clear(_hitBuffer, 0, colliderCount);
        }

        public void CloseHitWindow()
        {
            IsHitWindowActive = false;
        }

        public void CompleteAttack()
        {
            IsHitWindowActive = false;
            IsAttackInProgress = false;
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            TryStartAttack();
        }

        private void OnActorDied(ActorHealth actor)
        {
            CancelAttack();
        }

        private void CancelAttack()
        {
            IsHitWindowActive = false;
            IsAttackInProgress = false;
            if (_animator != null)
            {
                _animator.ResetTrigger(AttackTrigger);
                if (_animator.isActiveAndEnabled && _animator.runtimeAnimatorController != null)
                {
                    _animator.Play(IdleState, 0, 0f);
                }
            }
        }
    }
}
