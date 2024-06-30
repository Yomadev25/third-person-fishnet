using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public enum State
    {
        IDLE,
        CHASE,
        ATTACK
    }

    [SerializeField]
    private NavMeshAgent _navMesh;
    [SerializeField]
    private State _currentState;

    [Header("AI Configuration")]
    [SerializeField]
    private float _chaseRadius;
    [SerializeField]
    private float _attackRadius;

    [SerializeField]
    private Transform _playerTransform;

    private void Start()
    {
        //_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        ChangeState(State.IDLE);
    }

    private void Update()
    {
        UpdateState();
    }

    #region STATE MACHINE HANDLER
    private void ChangeState(State state)
    {
        if (state == _currentState) return;

        ExitState();
        _currentState = state;
        EnterState();
    }

    private void EnterState() //void start for state machine
    {
        switch (_currentState)
        {
            case State.IDLE:
                EnterIdleState();
                break;
            case State.CHASE:
                EnterChaseState();
                break;
            case State.ATTACK:
                EnterAttackState();
                break;
            default:
                break;
        }
    }

    private void UpdateState() //void update for state machine
    {
        switch (_currentState)
        {
            case State.IDLE:
                UpdateIdleState();
                break;
            case State.CHASE:
                UpdateChaseState();
                break;
            case State.ATTACK:
                UpdateAttackState();
                break;
            default:
                break;
        }
    }

    private void ExitState()
    {
        switch (_currentState)
        {
            case State.IDLE:
                ExitIdleState();
                break;
            case State.CHASE:
                ExitChaseState();
                break;
            case State.ATTACK:
                ExitAttackState();
                break;
            default:
                break;
        }
    }
    #endregion

    #region IDLE STATE
    private void EnterIdleState()
    {
        _navMesh.isStopped = true;
    }

    private void UpdateIdleState()
    {
        if (Vector3.Distance(transform.position, _playerTransform.position) < _chaseRadius)
        {
            ChangeState(State.CHASE);
        }
    }

    private void ExitIdleState()
    {
        
    }
    #endregion

    #region CHASE STATE
    private void EnterChaseState()
    {
        _navMesh.isStopped = false;
    }

    private void UpdateChaseState()
    {
        _navMesh.SetDestination(_playerTransform.position);

        if (Vector3.Distance(transform.position, _playerTransform.position) > _chaseRadius)
        {
            ChangeState(State.IDLE);
        }

        if (Vector3.Distance(transform.position, _playerTransform.position) < _attackRadius)
        {
            ChangeState(State.ATTACK);
        }
    }

    private void ExitChaseState()
    {
        
    }
    #endregion

    #region ATTACK STATE
    private void EnterAttackState()
    {
        _navMesh.isStopped = true;
        StartCoroutine(AttackCoroutine());
    }

    private void UpdateAttackState()
    {

    }

    private void ExitAttackState()
    {

    }

    IEnumerator AttackCoroutine()
    {
        Debug.Log("Attack");
        yield return new WaitForSeconds(3f);

        ChangeState(State.IDLE);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
}
