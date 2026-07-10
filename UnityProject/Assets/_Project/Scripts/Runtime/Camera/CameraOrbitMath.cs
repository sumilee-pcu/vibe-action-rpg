using UnityEngine;

namespace TinyVanguard.CameraControl
{
    public static class CameraOrbitMath
    {
        public static Vector2 GetNextOrbit(
            Vector2 currentOrbit,
            Vector2 lookDelta,
            Vector2 sensitivity,
            float minimumVerticalAngle,
            float maximumVerticalAngle)
        {
            if (minimumVerticalAngle > maximumVerticalAngle)
            {
                (minimumVerticalAngle, maximumVerticalAngle) =
                    (maximumVerticalAngle, minimumVerticalAngle);
            }

            var horizontal = Mathf.Repeat(
                currentOrbit.x + (lookDelta.x * Mathf.Max(0f, sensitivity.x)) + 180f,
                360f) - 180f;
            var vertical = Mathf.Clamp(
                currentOrbit.y + (lookDelta.y * Mathf.Max(0f, sensitivity.y)),
                minimumVerticalAngle,
                maximumVerticalAngle);

            return new Vector2(horizontal, vertical);
        }
    }
}
