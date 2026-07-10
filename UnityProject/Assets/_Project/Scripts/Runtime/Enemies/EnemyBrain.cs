using System;
using TinyVanguard.Combat;
using UnityEngine;

namespace TinyVanguard.Enemies
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyNavigationController))]
    [RequireComponent(typeof(ActorHealth))]
    public sealed class EnemyBrain : MonoBehaviour
    {
        [SerializeField] private EnemyDefinition _definition = null!;
        [SerializeField] private EnemyNavigationController _navigation = null!;
        [SerializeField] private ActorHealth _selfHealth = null!;
        [SerializeField] private Transform _target = null!;
        [SerializeField] private ActorHealth _targetHealth = null!;

        private EnemyStateMachine _stateMachine = null!;
        private bool _attackFinished;
        private bool _deathHandled;
        private double _nextAttackAt;

        public event Action<EnemyStateTransition> StateChanged = delegate { };
        public event Action<DamageResult> AttackResolved = delegate { };
        public event Action<int> RewardAvailable = delegate { };

        public EnemyDefinition Definition => _definition;
        public EnemyState CurrentState => _stateMachine?.CurrentState ?? EnemyState.Idle;
        public Vector3 HomePosition { get; private set; }
        public int AttackExecutionCount { get; private set; }
        public int AppliedAttackCount { get; private set; }
        public int DeathHandledCount { get; private set; }
        public int RewardSignalCount { get; private set; }

        public void Configure(
            EnemyDefinition definition,
            EnemyNavigationController navigation,
            ActorHealth selfHealth,
            Transform target,
            ActorHealth targetHealth)
        {
            _definition = definition;
            _navigation = navigation;
            _selfHealth = selfHealth;
            _target = target;
            _targetHealth = targetHealth;
        }

        public float GetRemainingCooldown(double now)
        {
            return Mathf.Max(0f, (float)(_nextAttackAt - now));
        }

        public void Tick(double now)
        {
            if (_stateMachine == null
                || _definition == null
                || _navigation == null
                || _selfHealth == null)
            {
                return;
            }

            var targetIsLiving = _target != null
                && (_targetHealth == null || _targetHealth.CanAct);
            var targetDistance = targetIsLiving
                ? Vector3.Distance(transform.position, _target.position)
                : float.PositiveInfinity;
            var targetDistanceFromHome = targetIsLiving
                ? Vector3.Distance(HomePosition, _target.position)
                : float.PositiveInfinity;
            var distanceFromHome = Vector3.Distance(transform.position, HomePosition);
            var hasDetectedTarget = targetIsLiving
                && (CurrentState != EnemyState.Idle
                    || targetDistance <= _definition.DetectionRange);
            var attackFinished = _attackFinished;
            _attackFinished = false;
            var previousState = CurrentState;

            _stateMachine.Tick(new EnemyStateSignals(
                isDead: !_selfHealth.CanAct,
                hasLivingTarget: hasDetectedTarget,
                isTargetInAttackRange: targetDistance <= _definition.AttackRange,
                canAttack: now >= _nextAttackAt,
                shouldDisengage: targetDistanceFromHome > _definition.DisengageRange,
                isAtHome: distanceFromHome <= _definition.HomeTolerance,
                hasFinishedAttack: attackFinished));

            switch (CurrentState)
            {
                case EnemyState.Idle:
                case EnemyState.Hit:
                case EnemyState.Dead:
                    _navigation.Stop();
                    break;

                case EnemyState.Chase:
                    if (targetIsLiving)
                    {
                        _navigation.TryMoveTo(_target.position);
                    }
                    break;

                case EnemyState.Return:
                    _navigation.TryMoveTo(HomePosition, _definition.HomeTolerance);
                    break;

                case EnemyState.Attack:
                    _navigation.Stop();
                    if (previousState != EnemyState.Attack)
                    {
                        ExecuteAttack(now);
                    }
                    break;
            }
        }

        private void Awake()
        {
            if (_navigation == null)
            {
                _navigation = GetComponent<EnemyNavigationController>();
            }

            if (_selfHealth == null)
            {
                _selfHealth = GetComponent<ActorHealth>();
            }

            HomePosition = transform.position;
            _stateMachine = new EnemyStateMachine();
            _stateMachine.StateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            if (_stateMachine != null)
            {
                _stateMachine.StateChanged -= OnStateChanged;
            }
        }

        private void OnEnable()
        {
            if (_selfHealth != null)
            {
                _selfHealth.Died += OnSelfDied;
                if (_selfHealth.State != null && _selfHealth.State.IsDead)
                {
                    OnSelfDied(_selfHealth);
                }
            }
        }

        private void Update()
        {
            Tick(Time.timeAsDouble);
        }

        private void OnDisable()
        {
            if (_selfHealth != null)
            {
                _selfHealth.Died -= OnSelfDied;
            }

            if (_navigation != null)
            {
                _navigation.Stop();
            }
        }

        private void ExecuteAttack(double now)
        {
            AttackExecutionCount++;
            _nextAttackAt = now + _definition.AttackCooldown;
            _attackFinished = true;

            var damage = _definition.Attack.BaseDamage + _definition.Actor.AttackPower;
            var result = _targetHealth != null
                ? _targetHealth.ApplyDamage(damage, _target.position)
                : default;
            if (result.WasApplied)
            {
                AppliedAttackCount++;
            }
            AttackResolved(result);
        }

        private void OnStateChanged(EnemyStateTransition transition)
        {
            StateChanged(transition);
        }

        private void OnSelfDied(ActorHealth actor)
        {
            if (_deathHandled)
            {
                return;
            }

            _deathHandled = true;
            DeathHandledCount++;
            _stateMachine.Tick(new EnemyStateSignals(isDead: true));
            _navigation.Stop();
            RewardSignalCount++;
            RewardAvailable(_definition.ExperienceReward);
        }
    }
}
