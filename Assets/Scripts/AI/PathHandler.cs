using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class PathHandler : MonoBehaviour
    {
        [Header("AI")] 
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField, Min(0)] private int currentPatrolPoint;
        
        private NavMeshAgent _agent;
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            currentPatrolPoint -= 1;
        }

        public bool HasReachedDestination()
        {
            return _agent.remainingDistance <= _agent.stoppingDistance;
        }
        
        public void SetNextPatrolPoint()
        {
            _agent.SetDestination(patrolPoints[++currentPatrolPoint % patrolPoints.Length].position);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
        
            for (int i = 0; i < patrolPoints.Length; ++i)
            {
                Gizmos.DrawSphere(patrolPoints[i].position, 0.25f);
                if (i == 0)
                {
                    Gizmos.DrawLine(patrolPoints[0].position, patrolPoints[^1].position);
                    continue;
                }

                Gizmos.DrawLine(patrolPoints[i-1].position, patrolPoints[i].position);
            }
        }
    }
}
