using System.Collections;
using System.Threading;
using UnityEngine;

public class PawnDeadState : PawnState
{
    public PawnDeadState(Enemy pawn, PawnStateMachine pawnStateMachine) : base(pawn, pawnStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}