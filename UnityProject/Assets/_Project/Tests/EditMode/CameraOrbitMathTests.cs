using NUnit.Framework;
using TinyVanguard.CameraControl;
using UnityEngine;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class CameraOrbitMathTests
    {
        [Test]
        public void LookDeltaIsScaledByIndependentSensitivity()
        {
            var next = CameraOrbitMath.GetNextOrbit(
                Vector2.zero,
                new Vector2(100f, 50f),
                new Vector2(0.12f, 0.08f),
                -20f,
                65f);

            Assert.That(next.x, Is.EqualTo(12f).Within(0.0001f));
            Assert.That(next.y, Is.EqualTo(4f).Within(0.0001f));
        }

        [Test]
        public void VerticalAngleClampsAtMaximum()
        {
            var next = CameraOrbitMath.GetNextOrbit(
                new Vector2(0f, 60f),
                new Vector2(0f, 100f),
                Vector2.one,
                -20f,
                65f);

            Assert.That(next.y, Is.EqualTo(65f));
        }

        [Test]
        public void VerticalAngleClampsAtMinimum()
        {
            var next = CameraOrbitMath.GetNextOrbit(
                new Vector2(0f, -15f),
                new Vector2(0f, -100f),
                Vector2.one,
                -20f,
                65f);

            Assert.That(next.y, Is.EqualTo(-20f));
        }

        [Test]
        public void HorizontalAngleWrapsAcrossPositiveBoundary()
        {
            var next = CameraOrbitMath.GetNextOrbit(
                new Vector2(175f, 20f),
                new Vector2(10f, 0f),
                Vector2.one,
                -20f,
                65f);

            Assert.That(next.x, Is.EqualTo(-175f).Within(0.0001f));
        }

        [Test]
        public void ReversedVerticalLimitsAreNormalized()
        {
            var next = CameraOrbitMath.GetNextOrbit(
                new Vector2(0f, 20f),
                new Vector2(0f, 100f),
                Vector2.one,
                65f,
                -20f);

            Assert.That(next.y, Is.EqualTo(65f));
        }

        [Test]
        public void NegativeSensitivityDoesNotReverseInput()
        {
            var next = CameraOrbitMath.GetNextOrbit(
                new Vector2(15f, 20f),
                new Vector2(100f, 100f),
                new Vector2(-1f, -1f),
                -20f,
                65f);

            Assert.That(next, Is.EqualTo(new Vector2(15f, 20f)));
        }
    }
}
