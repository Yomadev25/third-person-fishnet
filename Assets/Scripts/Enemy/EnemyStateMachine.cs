using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _navMesh;

    [Header("AI Configuration")]
    [SerializeField]
    private float _chaseRadius;
    [SerializeField]
    private float _attackRadius;

    public Transform playerTransform;

    public EnemyBaseState CurrentState;
    public EnemyStateFactory stateFactory;

    public NavMeshAgent navMesh => _navMesh;
    public float chaseRadius => _chaseRadius;

    private void Start()
    {
        stateFactory = new EnemyStateFactory(this);

        CurrentState = stateFactory.Idle();
        CurrentState.Enter();
    }

    private void Update()
    {
        CurrentState.Update();
    }

    private void FixedUpdate()
    {
        CurrentState.FixedUpdate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
}

public class EnemyStateFactory
{
    EnemyStateMachine _context;

    public EnemyStateFactory(EnemyStateMachine currentContext)
    {
        _context = currentContext;
    }

    public EnemyIdleState Idle()
    {
        return new EnemyIdleState(_context);
    }

    public EnemyChaseState Chase()
    {
        return new EnemyChaseState(_context);
    }
}


