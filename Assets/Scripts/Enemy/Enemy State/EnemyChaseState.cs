using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyStateMachine ctx) : base(ctx)
    {
    }

    public override void Enter()
    {
        _context.navMesh.isStopped = false;
    }

    public override void Exit()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        _context.navMesh.SetDestination(_context.playerTransform.position);

        if (Vector3.Distance(_context.transform.position, _context.playerTransform.position) > _context.chaseRadius)
        {
            ChangeState(_context.stateFactory.Idle());
        }
    }
}
