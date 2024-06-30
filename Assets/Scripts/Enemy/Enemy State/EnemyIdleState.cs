using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateMachine ctx) : base(ctx)
    {
    }

    public override void Enter()
    {
        _context.navMesh.isStopped = true;
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        if (Vector3.Distance(_context.transform.position, _context.playerTransform.position) < _context.chaseRadius)
        {
            ChangeState(_context.stateFactory.Chase());
        }
    }
}
