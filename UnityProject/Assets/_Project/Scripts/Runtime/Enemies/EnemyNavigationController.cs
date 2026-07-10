using UnityEngine;
using UnityEngine.AI;

namespace TinyVanguard.Enemies
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EnemyNavigationController : MonoBehaviour
    {
        [SerializeField] private EnemyDefinition _definition = null!;
        [SerializeField] private NavMeshAgent _agent = null!;

        public EnemyDefinition Definition => _definition;
        public NavMeshAgent Agent => _agent;
        public bool IsOnNavMesh => _agent != null && _agent.isOnNavMesh;

        public void Configure(EnemyDefinition definition, NavMeshAgent agent)
        {
            _definition = definition;
            _agent = agent;
            ApplyDefinition();
        }

        public bool TryMoveTo(Vector3 destination)
        {
            var stoppingDistance = _definition != null
                ? _definition.NavigationStoppingDistance
                : _agent.stoppingDistance;
            return TryMoveTo(destination, stoppingDistance);
        }

        public bool TryMoveTo(Vector3 destination, float stoppingDistance)
        {
            if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            {
                return false;
            }

            _agent.stoppingDistance = Mathf.Max(0f, stoppingDistance);
            _agent.isStopped = false;
            return _agent.SetDestination(destination);
        }

        public void Stop()
        {
            if (_agent == null || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            {
                return;
            }

            _agent.ResetPath();
            if (_definition != null)
            {
                _agent.stoppingDistance = _definition.NavigationStoppingDistance;
            }
            _agent.isStopped = true;
        }

        private void Awake()
        {
            if (_agent == null)
            {
                _agent = GetComponent<NavMeshAgent>();
            }

            ApplyDefinition();
        }

        private void ApplyDefinition()
        {
            if (_definition == null || _agent == null)
            {
                return;
            }

            _agent.speed = _definition.MoveSpeed;
            _agent.stoppingDistance = _definition.NavigationStoppingDistance;
            _agent.radius = Mathf.Max(0.1f, _definition.SeparationRadius * 0.5f);
            _agent.autoBraking = true;
        }
    }
}
