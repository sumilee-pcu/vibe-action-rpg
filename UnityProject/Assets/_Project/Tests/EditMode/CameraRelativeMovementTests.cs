using NUnit.Framework;
using TinyVanguard.Player;
using UnityEngine;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class CameraRelativeMovementTests
    {
        [Test]
        public void ForwardInputUsesCameraPlanarForward()
        {
            var direction = CameraRelativeMovement.GetWorldDirection(
                Vector2.up,
                Vector3.right);

            Assert.That(Vector3.Distance(direction, Vector3.right), Is.LessThan(0.0001f));
        }

        [Test]
        public void CameraPitchDoesNotTiltMovement()
        {
            var pitchedForward = new Vector3(0f, -1f, 1f).normalized;
            var direction = CameraRelativeMovement.GetWorldDirection(
                Vector2.up,
                pitchedForward);

            Assert.That(Mathf.Abs(direction.y), Is.LessThan(0.0001f));
            Assert.That(Vector3.Distance(direction, Vector3.forward), Is.LessThan(0.0001f));
        }

        [Test]
        public void DiagonalInputDoesNotExceedUnitMagnitude()
        {
            var direction = CameraRelativeMovement.GetWorldDirection(
                Vector2.one,
                Vector3.forward);

            Assert.That(direction.magnitude, Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void EqualDurationProducesEqualDistanceAcrossFrameRates()
        {
            var atThirtyFramesPerSecond = SimulateTravel(30, 2f, 6f);
            var atOneHundredTwentyFramesPerSecond = SimulateTravel(120, 2f, 6f);

            Assert.That(
                Vector3.Distance(atThirtyFramesPerSecond, atOneHundredTwentyFramesPerSecond),
                Is.LessThan(0.0001f));
            Assert.That(atThirtyFramesPerSecond.magnitude, Is.EqualTo(12f).Within(0.0001f));
        }

        [Test]
        public void FacingRotatesAtDegreesPerSecond()
        {
            var rotated = CameraRelativeMovement.RotateTowardsMovement(
                Quaternion.identity,
                Vector3.right,
                90f,
                0.5f);
            var target = Quaternion.LookRotation(Vector3.right, Vector3.up);

            Assert.That(Quaternion.Angle(rotated, target), Is.EqualTo(45f).Within(0.001f));
        }

        [Test]
        public void ZeroDirectionKeepsCurrentFacing()
        {
            var current = Quaternion.Euler(0f, 37f, 0f);
            var rotated = CameraRelativeMovement.RotateTowardsMovement(
                current,
                Vector3.zero,
                720f,
                1f);

            Assert.That(Quaternion.Angle(rotated, current), Is.LessThan(0.0001f));
        }

        private static Vector3 SimulateTravel(int framesPerSecond, float seconds, float speed)
        {
            var position = Vector3.zero;
            var frameCount = Mathf.RoundToInt(framesPerSecond * seconds);
            var deltaTime = 1f / framesPerSecond;

            for (var frame = 0; frame < frameCount; frame++)
            {
                position += CameraRelativeMovement.GetDisplacement(
                    Vector3.forward,
                    speed,
                    deltaTime);
            }

            return position;
        }
    }
}
