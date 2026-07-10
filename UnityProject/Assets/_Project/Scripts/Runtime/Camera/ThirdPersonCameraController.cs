using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyVanguard.CameraControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineOrbitalFollow))]
    public sealed class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputActionAsset _inputActions = null!;
        [SerializeField] private CinemachineOrbitalFollow _orbitalFollow = null!;

        [Header("Look")]
        [SerializeField, Min(0f)] private float _horizontalSensitivity = 0.12f;
        [SerializeField, Min(0f)] private float _verticalSensitivity = 0.08f;
        [SerializeField] private float _minimumVerticalAngle = -20f;
        [SerializeField] private float _maximumVerticalAngle = 65f;
        [SerializeField] private bool _invertVertical;

        [Header("Cursor")]
        [SerializeField] private bool _lockCursorOnFocus = true;

        private InputActionMap _gameplayMap = null!;
        private InputAction _lookAction = null!;
        private bool _lockedCursor;

        public InputActionAsset InputActions => _inputActions;
        public CinemachineOrbitalFollow OrbitalFollow => _orbitalFollow;
        public Vector2 Sensitivity =>
            new(_horizontalSensitivity, _verticalSensitivity);
        public Vector2 VerticalLimits =>
            new(_minimumVerticalAngle, _maximumVerticalAngle);

        public void Configure(
            InputActionAsset inputActions,
            CinemachineOrbitalFollow orbitalFollow,
            Vector2 sensitivity,
            Vector2 verticalLimits,
            bool lockCursorOnFocus)
        {
            _inputActions = inputActions;
            _orbitalFollow = orbitalFollow;
            _horizontalSensitivity = Mathf.Max(0f, sensitivity.x);
            _verticalSensitivity = Mathf.Max(0f, sensitivity.y);
            _minimumVerticalAngle = Mathf.Min(verticalLimits.x, verticalLimits.y);
            _maximumVerticalAngle = Mathf.Max(verticalLimits.x, verticalLimits.y);
            _lockCursorOnFocus = lockCursorOnFocus;
            ApplyAxisSettings();
        }

        private void Awake()
        {
            if (_orbitalFollow == null)
            {
                _orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
            }

            if (_inputActions == null)
            {
                _inputActions = InputSystem.actions;
            }

            _gameplayMap = _inputActions?.FindActionMap("Gameplay");
            _lookAction = _gameplayMap?.FindAction("Look");
            if (_gameplayMap == null || _lookAction == null)
            {
                Debug.LogError(
                    $"[{nameof(ThirdPersonCameraController)}] Missing Gameplay/Look input.",
                    this);
                enabled = false;
                return;
            }

            ApplyAxisSettings();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _gameplayMap?.Enable();
            ApplyCursorLock(Application.isFocused);
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ReleaseCursor();
        }

        private void Update()
        {
            if (_lookAction == null || _orbitalFollow == null)
            {
                return;
            }

            var lookDelta = _lookAction.ReadValue<Vector2>();
            if (_invertVertical)
            {
                lookDelta.y = -lookDelta.y;
            }

            var currentOrbit = new Vector2(
                _orbitalFollow.HorizontalAxis.Value,
                _orbitalFollow.VerticalAxis.Value);
            var nextOrbit = CameraOrbitMath.GetNextOrbit(
                currentOrbit,
                lookDelta,
                Sensitivity,
                _minimumVerticalAngle,
                _maximumVerticalAngle);

            _orbitalFollow.HorizontalAxis.Value = nextOrbit.x;
            _orbitalFollow.VerticalAxis.Value = nextOrbit.y;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ApplyCursorLock(hasFocus);
        }

        private void OnValidate()
        {
            _horizontalSensitivity = Mathf.Max(0f, _horizontalSensitivity);
            _verticalSensitivity = Mathf.Max(0f, _verticalSensitivity);
            if (_minimumVerticalAngle > _maximumVerticalAngle)
            {
                (_minimumVerticalAngle, _maximumVerticalAngle) =
                    (_maximumVerticalAngle, _minimumVerticalAngle);
            }

            if (_orbitalFollow == null)
            {
                _orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
            }

            ApplyAxisSettings();
        }

        private void ApplyAxisSettings()
        {
            if (_orbitalFollow == null)
            {
                return;
            }

            var horizontalAxis = _orbitalFollow.HorizontalAxis;
            horizontalAxis.Range = new Vector2(-180f, 180f);
            horizontalAxis.Wrap = true;
            horizontalAxis.Value = horizontalAxis.ClampValue(horizontalAxis.Value);
            _orbitalFollow.HorizontalAxis = horizontalAxis;

            var verticalAxis = _orbitalFollow.VerticalAxis;
            verticalAxis.Range = new Vector2(
                _minimumVerticalAngle,
                _maximumVerticalAngle);
            verticalAxis.Wrap = false;
            verticalAxis.Value = verticalAxis.ClampValue(verticalAxis.Value);
            _orbitalFollow.VerticalAxis = verticalAxis;
        }

        private void ApplyCursorLock(bool hasFocus)
        {
            if (!_lockCursorOnFocus || !hasFocus || Application.isBatchMode)
            {
                ReleaseCursor();
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _lockedCursor = true;
        }

        private void ReleaseCursor()
        {
            if (!_lockedCursor)
            {
                return;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _lockedCursor = false;
        }
    }
}
