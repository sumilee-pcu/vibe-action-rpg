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

        private CharacterController _characterController = null!;
        private InputActionMap _gameplayMap = null!;
        private InputAction _moveAction = null!;
        private float _verticalVelocity;

        public Transform CameraTransform => _cameraTransform;
        public InputActionAsset InputActions => _inputActions;
        public float MoveSpeed => _moveSpeed;
        public float TurnSpeedDegreesPerSecond => _turnSpeedDegreesPerSecond;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            if (_inputActions == null)
            {
                _inputActions = InputSystem.actions;
            }

            _gameplayMap = _inputActions?.FindActionMap("Gameplay");
            _moveAction = _gameplayMap?.FindAction("Move");
            if (_gameplayMap == null || _moveAction == null)
            {
                Debug.LogError(
                    $"[{nameof(PlayerMovementController)}] Missing Gameplay/Move input.",
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

        private void OnEnable()
        {
            if (_gameplayMap != null)
            {
                _gameplayMap.Enable();
            }
        }

        private void OnDisable()
        {
            _gameplayMap?.Disable();
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

            UpdateVerticalVelocity(deltaTime);

            var horizontalDisplacement = CameraRelativeMovement.GetDisplacement(
                worldDirection,
                _moveSpeed,
                deltaTime);
            var verticalDisplacement = Vector3.up * (_verticalVelocity * deltaTime);
            _characterController.Move(horizontalDisplacement + verticalDisplacement);

            transform.rotation = CameraRelativeMovement.RotateTowardsMovement(
                transform.rotation,
                worldDirection,
                _turnSpeedDegreesPerSecond,
                deltaTime);
        }

        private void OnValidate()
        {
            _moveSpeed = Mathf.Max(0f, _moveSpeed);
            _turnSpeedDegreesPerSecond = Mathf.Max(0f, _turnSpeedDegreesPerSecond);
            _groundedVerticalVelocity = Mathf.Min(0f, _groundedVerticalVelocity);
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
