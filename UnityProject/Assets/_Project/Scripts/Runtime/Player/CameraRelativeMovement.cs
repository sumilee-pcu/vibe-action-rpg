using UnityEngine;

namespace TinyVanguard.Player
{
    public static class CameraRelativeMovement
    {
        private const float DirectionEpsilon = 0.0001f;

        public static Vector3 GetWorldDirection(Vector2 input, Vector3 cameraForward)
        {
            var planarForward = Vector3.ProjectOnPlane(cameraForward, Vector3.up);
            if (planarForward.sqrMagnitude < DirectionEpsilon)
            {
                planarForward = Vector3.forward;
            }

            planarForward.Normalize();
            var planarRight = Vector3.Cross(Vector3.up, planarForward);
            var clampedInput = Vector2.ClampMagnitude(input, 1f);
            var worldDirection =
                (planarRight * clampedInput.x) + (planarForward * clampedInput.y);

            return Vector3.ClampMagnitude(worldDirection, 1f);
        }

        public static Vector3 GetDisplacement(
            Vector3 worldDirection,
            float speed,
            float deltaTime)
        {
            var safeSpeed = Mathf.Max(0f, speed);
            var safeDeltaTime = Mathf.Max(0f, deltaTime);
            return worldDirection * safeSpeed * safeDeltaTime;
        }

        public static Quaternion RotateTowardsMovement(
            Quaternion currentRotation,
            Vector3 worldDirection,
            float degreesPerSecond,
            float deltaTime)
        {
            var planarDirection = Vector3.ProjectOnPlane(worldDirection, Vector3.up);
            if (planarDirection.sqrMagnitude < DirectionEpsilon)
            {
                return currentRotation;
            }

            var targetRotation = Quaternion.LookRotation(planarDirection.normalized, Vector3.up);
            var maximumDegreesDelta =
                Mathf.Max(0f, degreesPerSecond) * Mathf.Max(0f, deltaTime);

            return Quaternion.RotateTowards(
                currentRotation,
                targetRotation,
                maximumDegreesDelta);
        }
    }
}
