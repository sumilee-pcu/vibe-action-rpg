using NUnit.Framework;
using TinyVanguard.Enemies;
using UnityEngine;

namespace TinyVanguard.Tests.EditMode
{
    public sealed class EnemyApproachSlotsTests
    {
        [Test]
        public void FiveSlotsMaintainDistinctPositionsAroundTarget()
        {
            const int slotCount = 5;
            const float radius = 1.25f;
            var positions = new Vector3[slotCount];

            for (var index = 0; index < slotCount; index++)
            {
                positions[index] = EnemyApproachSlots.GetPosition(
                    Vector3.zero,
                    index,
                    slotCount,
                    radius);
                Assert.That(positions[index].magnitude, Is.EqualTo(radius).Within(0.001f));
            }

            for (var first = 0; first < slotCount; first++)
            {
                for (var second = first + 1; second < slotCount; second++)
                {
                    Assert.That(
                        Vector3.Distance(positions[first], positions[second]),
                        Is.GreaterThan(1f));
                }
            }
        }
    }
}
