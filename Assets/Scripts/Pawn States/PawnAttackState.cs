using System.Collections;
using System.Threading;
using UnityEngine;

public class PawnAttackState : PawnState
{
    public PawnAttackState(Enemy pawn, PawnStateMachine pawnStateMachine) : base(pawn, pawnStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
        pawn.StartCoroutine(Attack());
        pawn.StartCoroutine(WaitForAttackToFinish());
    }

    public override void ExitState()
    {
        base.ExitState();

        pawn.StopAllCoroutines();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (!pawn.inAttackRange)
        {
            pawn.StateMachine.ChangeState(pawn.ChaseState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private IEnumerator WaitForAttackToFinish()
    {
        yield return new WaitUntil(() =>
            pawn.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        );

        yield return new WaitUntil(() =>
            !pawn.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        );

    }

    private IEnumerator Attack()
    {
        while (pawn.inAttackRange)
        {
            yield return new WaitForSeconds(1f);
            pawn.animator.SetTrigger("attack");
        }
    }
}