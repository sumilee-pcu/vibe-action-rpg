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

        [SerializeField] private InputActionAsset _inputActions = null!;
        [SerializeField] private AttackDefinition _attackDefinition = null!;
        [SerializeField] private Animator _animator = null!;

        private InputActionMap _gameplayMap = null!;
        private InputAction _attackAction = null!;

        public InputActionAsset InputActions => _inputActions;
        public AttackDefinition AttackDefinition => _attackDefinition;
        public Animator Animator => _animator;
        public bool IsAttackInProgress { get; private set; }
        public bool IsHitWindowActive { get; private set; }
        public int AttackSequence { get; private set; }

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
        }

        private void OnDisable()
        {
            if (_attackAction != null)
            {
                _attackAction.performed -= OnAttackPerformed;
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
                || IsAttackInProgress)
            {
                return false;
            }

            IsAttackInProgress = true;
            IsHitWindowActive = false;
            AttackSequence++;
            _animator.ResetTrigger(AttackTrigger);
            _animator.SetTrigger(AttackTrigger);
            return true;
        }

        public void OpenHitWindow()
        {
            if (IsAttackInProgress)
            {
                IsHitWindowActive = true;
            }
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
