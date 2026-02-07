using System.Collections;
using System.Threading;
using UnityEngine;

public class PawnChaseState : PawnState
{
    protected Vector2 targetPlayer;
    protected float pawnSpeed = 2.0f;
    public PawnChaseState(Enemy pawn, PawnStateMachine pawnStateMachine) : base(pawn, pawnStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("i see you");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //raptor-x-z
        if (pawn.player != null)
        {
            //Gets player position
            targetPlayer = pawn.player.transform.position;

            if (pawn.player.transform.position.x < pawn.transform.position.x)
            {
                pawn.transform.localScale = Vector3.one;
            }

            if (pawn.player.transform.position.x > pawn.transform.position.x)
            {
                pawn.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        if (pawn.inAttackRange)
        {
            pawn.StateMachine.ChangeState(pawn.AttackState);
        }

        else if (pawn.canMove)
        {
            //Updates position to match with the player
            pawn.transform.position = Vector2.MoveTowards(pawn.transform.position, new Vector3(pawn.player.transform.position.x, pawn.transform.position.y, pawn.transform.position.z), pawnSpeed * Time.deltaTime);
        }   

        //Checking if Pawn hits the range limit
        if (pawn.transform.position.x < pawn.leftLimit.x + 0.5)
        {
            pawn.StateMachine.ChangeState(pawn.IdleState);
        }

        if (pawn.transform.position.x > pawn.rightLimit.x - 0.5)
        {
            pawn.StateMachine.ChangeState(pawn.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}