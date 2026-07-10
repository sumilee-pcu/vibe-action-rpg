using System;
using UnityEngine;

namespace TinyVanguard.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ActorHealth))]
    public sealed class ActorReactionController : MonoBehaviour
    {
        public const string HitTrigger = "Hit";
        public const string DeathTrigger = "Die";

        [SerializeField] private ActorHealth _health = null!;
        [SerializeField] private Animator? _animator;
        [SerializeField] private Behaviour[] _disableOnDeath = Array.Empty<Behaviour>();

        private bool _isSubscribed;

        public bool IsDead { get; private set; }
        public int HitReactionCount { get; private set; }
        public int DeathTransitionCount { get; private set; }

        public void Configure(
            ActorHealth health,
            Animator? animator = null,
            Behaviour[]? disableOnDeath = null)
        {
            Unsubscribe();
            _health = health;
            _animator = animator;
            _disableOnDeath = disableOnDeath ?? Array.Empty<Behaviour>();

            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        private void Awake()
        {
            if (_health == null)
            {
                _health = GetComponent<ActorHealth>();
            }

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_isSubscribed || _health == null)
            {
                return;
            }

            _health.DamageApplied += OnDamageApplied;
            _health.Died += OnDied;
            _isSubscribed = true;

            if (_health.State != null && _health.State.IsDead)
            {
                TransitionToDeath();
            }
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || _health == null)
            {
                return;
            }

            _health.DamageApplied -= OnDamageApplied;
            _health.Died -= OnDied;
            _isSubscribed = false;
        }

        private void OnDamageApplied(DamageAppliedEvent damageEvent)
        {
            if (damageEvent.CausedDeath || IsDead)
            {
                return;
            }

            HitReactionCount++;
            if (_animator != null && _animator.runtimeAnimatorController != null)
            {
                _animator.SetTrigger(HitTrigger);
            }
        }

        private void OnDied(ActorHealth actor)
        {
            TransitionToDeath();
        }

        private void TransitionToDeath()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            DeathTransitionCount++;

            if (_animator != null && _animator.runtimeAnimatorController != null)
            {
                _animator.ResetTrigger(HitTrigger);
                _animator.SetTrigger(DeathTrigger);
            }

            foreach (var behaviour in _disableOnDeath)
            {
                if (behaviour != null && behaviour != this)
                {
                    behaviour.enabled = false;
                }
            }
        }
    }
}
