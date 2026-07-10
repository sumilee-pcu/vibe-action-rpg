using System;
using System.Collections.Generic;

namespace TinyVanguard.Combat
{
    public sealed class AttackExecution
    {
        private readonly HashSet<object> _registeredTargets = new();

        public AttackExecution(int sequence)
        {
            if (sequence <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sequence),
                    sequence,
                    "Attack sequence must be positive.");
            }

            Sequence = sequence;
        }

        public int Sequence { get; }
        public int RegisteredTargetCount => _registeredTargets.Count;

        public bool TryRegisterTarget(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return _registeredTargets.Add(target);
        }
    }
}
