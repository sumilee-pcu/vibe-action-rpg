using System;
using UnityEngine;

namespace TinyVanguard.Enemies
{
    public static class EnemyApproachSlots
    {
        public static Vector3 GetPosition(
            Vector3 center,
            int slotIndex,
            int slotCount,
            float radius)
        {
            if (slotCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slotCount));
            }

            if (slotIndex < 0 || slotIndex >= slotCount)
            {
                throw new ArgumentOutOfRangeException(nameof(slotIndex));
            }

            var angle = Mathf.PI * 2f * slotIndex / slotCount;
            var offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            return center + offset * Mathf.Max(0f, radius);
        }
    }
}
