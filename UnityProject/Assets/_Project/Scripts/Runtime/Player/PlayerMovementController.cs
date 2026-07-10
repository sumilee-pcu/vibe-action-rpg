using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyVanguard.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovementController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _cameraTransform = null!;
        [SerializeField] private InputActionAsset _inputActions = null!;

        [Header("Movement")]
        [SerializeField, Min(0f)] private float _moveSpeed = 5f;
        [SerializeField, Min(0f)] private float _turnSpeedDegreesPerSecond = 720f;
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private float _groundedVerticalVelocity = -2f;

        [Header("Dodge")]
        [SerializeField, Min(0f)] private float _dodgeDistance = 4f;
        [SerializeField, Min(0.01f)] private float _dodgeDuration = 0.35f;
        [SerializeField, Min(0f)] private float _invulnerabilityStart = 0.05f;
        [SerializeField, Min(0f)] private float _invulnerabilityEnd = 0.25f;

        private CharacterController _characterController = null!;
        private InputActionMap _gameplayMap = null!;
        private InputAction _moveAction = null!;
        private InputAction _dodgeAction = null!;
        private readonly PlayerDodgeState _dodgeState = new();
        private float _verticalVelocity;

        public Transform CameraTransform => _cameraTransform;
        public InputActionAsset InputActions => _inputActions;
        public float MoveSpeed => _moveSpeed;
        public float TurnSpeedDegreesPerSecond => _turnSpeedDegreesPerSecond;
        public float DodgeDistance => _dodgeDistance;
        public float DodgeDuration => _dodgeDuration;
        public float InvulnerabilityStart => _invulnerabilityStart;
        public float InvulnerabilityEnd => _invulnerabilityEnd;
        public bool IsDodging => _dodgeState.IsActive;
        public bool IsInvulnerable => _dodgeState.IsInvulnerable;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            if (_inputActions == null)
            {
                _inputActions = InputSystem.actions;
            }

            _gameplayMap = _inputActions?.FindActionMap("Gameplay");
            _moveAction = _gameplayMap?.FindAction("Move");
            _dodgeAction = _gameplayMap?.FindAction("Dodge");
            if (_gameplayMap == null || _moveAction == null || _dodgeAction == null)
            {
                Debug.LogError(
                    $"[{nameof(PlayerMovementController)}] Missing Gameplay/Move or Dodge input.",
                    this);
                enabled = false;
                return;
            }

            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }

            if (_cameraTransform == null)
            {
                Debug.LogError(
                    $"[{nameof(PlayerMovementController)}] A camera transform is required.",
                    this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (_moveAction == null)
            {
                return;
            }

            var deltaTime = Time.deltaTime;
            var input = _moveAction.ReadValue<Vector2>();
            var worldDirection = CameraRelativeMovement.GetWorldDirection(
                input,
                _cameraTransform.forward);

            if (!_gameplayMap.enabled)
            {
                _dodgeState.Cancel();
            }
            else if (_dodgeAction.WasPressedThisFrame() && !_dodgeState.IsActive)
            {
                var dodgeDirection = worldDirection.sqrMagnitude > 0f
                    ? worldDirection
                    : transform.forward;
                _dodgeState.TryStart(
                    dodgeDirection,
                    _dodgeDuration,
                    _invulnerabilityStart,
                    _invulnerabilityEnd);
            }

            UpdateVerticalVelocity(deltaTime);

            var horizontalDisplacement = _dodgeState.IsActive
                ? _dodgeState.Step(deltaTime, _dodgeDistance)
                : CameraRelativeMovement.GetDisplacement(
                    worldDirection,
                    _moveSpeed,
                    deltaTime);
            var verticalDisplacement = Vector3.up * (_verticalVelocity * deltaTime);
            _characterController.Move(horizontalDisplacement + verticalDisplacement);

            var facingDirection = _dodgeState.IsActive
                ? _dodgeState.Direction
                : worldDirection;
            transform.rotation = CameraRelativeMovement.RotateTowardsMovement(
                transform.rotation,
                facingDirection,
                _turnSpeedDegreesPerSecond,
                deltaTime);
        }

        private void OnValidate()
        {
            _moveSpeed = Mathf.Max(0f, _moveSpeed);
            _turnSpeedDegreesPerSecond = Mathf.Max(0f, _turnSpeedDegreesPerSecond);
            _groundedVerticalVelocity = Mathf.Min(0f, _groundedVerticalVelocity);
            _dodgeDistance = Mathf.Max(0f, _dodgeDistance);
            _dodgeDuration = Mathf.Max(0.01f, _dodgeDuration);
            _invulnerabilityStart = Mathf.Clamp(
                _invulnerabilityStart,
                0f,
                _dodgeDuration);
            _invulnerabilityEnd = Mathf.Clamp(
                _invulnerabilityEnd,
                _invulnerabilityStart,
                _dodgeDuration);
        }

        private void UpdateVerticalVelocity(float deltaTime)
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = _groundedVerticalVelocity;
                return;
            }

            _verticalVelocity += _gravity * deltaTime;
        }
    }
}
