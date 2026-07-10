using UnityEngine;

namespace TinyVanguard.Player
{
    public sealed class PlayerDodgeState
    {
        private Vector3 _direction;
        private float _duration;
        private float _invulnerabilityStart;
        private float _invulnerabilityEnd;
        private float _elapsed;

        public bool IsActive { get; private set; }
        public bool IsInvulnerable => IsActive
            && _elapsed >= _invulnerabilityStart
            && _elapsed < _invulnerabilityEnd;
        public float Elapsed => _elapsed;
        public Vector3 Direction => _direction;

        public bool TryStart(
            Vector3 direction,
            float duration,
            float invulnerabilityStart,
            float invulnerabilityEnd)
        {
            if (IsActive || duration <= 0f || direction.sqrMagnitude <= 0f)
            {
                return false;
            }

            _direction = direction.normalized;
            _duration = duration;
            _invulnerabilityStart = Mathf.Clamp(invulnerabilityStart, 0f, duration);
            _invulnerabilityEnd = Mathf.Clamp(
                invulnerabilityEnd,
                _invulnerabilityStart,
                duration);
            _elapsed = 0f;
            IsActive = true;
            return true;
        }

        public Vector3 Step(float deltaTime, float totalDistance)
        {
            if (!IsActive || deltaTime <= 0f)
            {
                return Vector3.zero;
            }

            var previousElapsed = _elapsed;
            _elapsed = Mathf.Min(_elapsed + deltaTime, _duration);
            var activeDelta = _elapsed - previousElapsed;
            var speed = Mathf.Max(0f, totalDistance) / _duration;
            var displacement = _direction * (speed * activeDelta);

            if (_elapsed >= _duration)
            {
                IsActive = false;
            }

            return displacement;
        }

        public void Cancel()
        {
            IsActive = false;
        }
    }
}
