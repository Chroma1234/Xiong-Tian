using System.Collections;
using System.Threading;
using UnityEngine;

public class PawnStunnedState : PawnState
{
    private float stunDuration;
    private Coroutine stunCoroutine;

    public PawnStunnedState(Enemy pawn, PawnStateMachine pawnStateMachine, float duration) : base(pawn, pawnStateMachine)
    {
        stunDuration = duration;
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log("im stunned!");

        pawn.canMove = false;
        pawn.Stunned();

        stunCoroutine = pawn.StartCoroutine(StunTimer());
    }

    public override void ExitState()
    {
        if (stunCoroutine != null)
            pawn.StopCoroutine(stunCoroutine);

        pawn.canMove = true;
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

    private IEnumerator StunTimer()
    {
        yield return new WaitForSeconds(stunDuration);

        if (pawn.inAttackRange)
            pawn.StateMachine.ChangeState(pawn.AttackState);
        else
            pawn.StateMachine.ChangeState(pawn.ChaseState);
    }
}