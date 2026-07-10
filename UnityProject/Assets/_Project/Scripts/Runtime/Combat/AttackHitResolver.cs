using UnityEngine;

namespace TinyVanguard.Combat
{
    public static class AttackHitResolver
    {
        public static int ApplyDamage(
            Collider[] colliders,
            AttackExecution execution,
            int damage)
        {
            if (colliders == null || execution == null || damage <= 0)
            {
                return 0;
            }

            var appliedTargetCount = 0;
            foreach (var collider in colliders)
            {
                if (collider == null)
                {
                    continue;
                }

                var target = collider.GetComponentInParent<ActorHealth>();
                if (target == null || !execution.TryRegisterTarget(target))
                {
                    continue;
                }

                if (target.ApplyDamage(damage, collider.bounds.center).WasApplied)
                {
                    appliedTargetCount++;
                }
            }

            return appliedTargetCount;
        }
    }
}
