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
            return ApplyDamage(
                colliders,
                colliders?.Length ?? 0,
                execution,
                damage);
        }

        public static int ApplyDamage(
            Collider[] colliders,
            int colliderCount,
            AttackExecution execution,
            int damage,
            ActorHealth? ignoredTarget = null)
        {
            if (colliders == null
                || colliderCount <= 0
                || execution == null
                || damage <= 0)
            {
                return 0;
            }

            var appliedTargetCount = 0;
            var count = Mathf.Min(colliderCount, colliders.Length);
            for (var index = 0; index < count; index++)
            {
                var collider = colliders[index];
                if (collider == null)
                {
                    continue;
                }

                var target = collider.GetComponentInParent<ActorHealth>();
                if (target == null
                    || target == ignoredTarget
                    || !execution.TryRegisterTarget(target))
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
