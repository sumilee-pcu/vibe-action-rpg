using UnityEngine;

namespace TinyVanguard.Combat
{
    public readonly struct DamageAppliedEvent
    {
        public DamageAppliedEvent(
            ActorHealth target,
            DamageResult result,
            Vector3 worldPosition)
        {
            Target = target;
            Result = result;
            WorldPosition = worldPosition;
        }

        public ActorHealth Target { get; }
        public DamageResult Result { get; }
        public Vector3 WorldPosition { get; }
        public int Amount => Result.AppliedDamage;
        public bool CausedDeath => Result.CausedDeath;
    }
}
