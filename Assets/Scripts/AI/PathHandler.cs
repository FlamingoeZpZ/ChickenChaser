using System;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    //These need to execute early
    [DefaultExecutionOrder(-100)]
    public class PathHandler : MonoBehaviour
    {
        [Header("AI")] 
        [SerializeField] private WayPoint[] patrolPoints;
        [SerializeField, Min(0)] private int currentPatrolPoint;
        
        private NavMeshAgent _agent;
        private WayPoint _currentWaypoint;
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            currentPatrolPoint -= 1;
        }

        public bool HasReachedDestination(out float suggestedDelay)
        {
            suggestedDelay = _currentWaypoint.SuggestedDelay;
            return _agent.remainingDistance <= _agent.stoppingDistance;
        }
        
        public void SetNextPatrolPoint()
        {
            _currentWaypoint = patrolPoints[++currentPatrolPoint % patrolPoints.Length];
            _agent.SetDestination(_currentWaypoint.Position);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
        
            for (int i = 0; i < patrolPoints.Length; ++i)
            {
                Gizmos.DrawSphere(patrolPoints[i].transform.position, 0.25f);
                if (i == 0)
                {
                    Gizmos.DrawLine(patrolPoints[0].transform.position, patrolPoints[^1].transform.position);
                    continue;
                }

                Gizmos.DrawLine(patrolPoints[i-1].transform.position, patrolPoints[i].transform.position);
            }
        }

        public Vector3 GetSuggestedForward()
        {
            return _currentWaypoint.Forward;
        }
    }
}
