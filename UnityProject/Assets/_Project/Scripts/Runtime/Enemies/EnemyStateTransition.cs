namespace TinyVanguard.Enemies
{
    public readonly struct EnemyStateTransition
    {
        public EnemyStateTransition(EnemyState previous, EnemyState current)
        {
            Previous = previous;
            Current = current;
        }

        public EnemyState Previous { get; }
        public EnemyState Current { get; }
    }
}
