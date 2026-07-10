using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyVanguard.Session
{
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class GameplayInputGate : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputActionAsset _inputActions = null!;

        [Header("Session")]
        [SerializeField] private GameSessionState _initialState = GameSessionState.Playing;
        [SerializeField] private bool _manageCursor = true;

        private InputActionMap _gameplayMap = null!;
        private InputActionMap _systemMap = null!;
        private InputActionMap _uiMap = null!;
        private InputAction _pauseAction = null!;
        private GameSessionState _state;
        private bool _initialized;

        public event Action<GameSessionState> StateChanged = delegate { };

        public InputActionAsset InputActions => _inputActions;
        public GameSessionState InitialState => _initialState;
        public GameSessionState State => _state;
        public bool IsGameplayInputEnabled =>
            _initialized && _gameplayMap.enabled;

        public void Configure(
            InputActionAsset inputActions,
            GameSessionState initialState,
            bool manageCursor)
        {
            _inputActions = inputActions;
            _initialState = initialState;
            _manageCursor = manageCursor;
            _state = initialState;

            if (Application.isPlaying)
            {
                _initialized = TryInitialize();
                if (_initialized && isActiveAndEnabled)
                {
                    ApplyState();
                }
            }
        }

        public bool SetState(GameSessionState state)
        {
            var changed = _state != state;
            _state = state;

            if (_initialized && isActiveAndEnabled)
            {
                ApplyState();
            }

            if (changed)
            {
                StateChanged(state);
            }

            return changed;
        }

        public bool TogglePause()
        {
            if (!GameplayInputPolicy.TryGetPauseToggleState(_state, out var next))
            {
                return false;
            }

            SetState(next);
            return true;
        }

        private void Awake()
        {
            _state = _initialState;
            _initialized = TryInitialize();
            if (!_initialized)
            {
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (!_initialized)
            {
                return;
            }

            _pauseAction.performed += OnPausePerformed;
            ApplyState();
        }

        private void OnDisable()
        {
            if (_initialized)
            {
                _pauseAction.performed -= OnPausePerformed;
                _gameplayMap.Disable();
                _systemMap.Disable();
                _uiMap.Disable();
            }

            ReleaseCursor();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (Application.isPlaying && isActiveAndEnabled)
            {
                ApplyCursorState(hasFocus);
            }
        }

        private bool TryInitialize()
        {
            if (_inputActions == null)
            {
                _inputActions = InputSystem.actions;
            }

            _gameplayMap = _inputActions?.FindActionMap("Gameplay");
            _systemMap = _inputActions?.FindActionMap("System");
            _uiMap = _inputActions?.FindActionMap("UI");
            _pauseAction = _systemMap?.FindAction("Pause");

            if (_gameplayMap != null
                && _systemMap != null
                && _uiMap != null
                && _pauseAction != null)
            {
                return true;
            }

            Debug.LogError(
                $"[{nameof(GameplayInputGate)}] Gameplay, System/Pause and UI input maps are required.",
                this);
            return false;
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        private void ApplyState()
        {
            var activation = GameplayInputPolicy.GetActivation(_state);

            SetMapEnabled(_gameplayMap, activation.Gameplay);
            SetMapEnabled(_uiMap, activation.UI);
            SetMapEnabled(_systemMap, activation.System);
            ApplyCursorState(Application.isFocused);
        }

        private static void SetMapEnabled(InputActionMap map, bool shouldEnable)
        {
            if (shouldEnable)
            {
                map.Enable();
            }
            else
            {
                map.Disable();
            }
        }

        private void ApplyCursorState(bool hasFocus)
        {
            if (!_manageCursor)
            {
                return;
            }

            if (_state == GameSessionState.Playing
                && hasFocus
                && !Application.isBatchMode)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return;
            }

            ReleaseCursor();
        }

        private void ReleaseCursor()
        {
            if (!_manageCursor)
            {
                return;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
