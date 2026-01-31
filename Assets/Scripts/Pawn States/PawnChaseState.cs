using System.Collections;
using System.Threading;
using UnityEngine;

public class PawnChaseState : PawnState
{
    protected Vector2 targetPlayer;
    protected float pawnSpeed = 1.0f;
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

        if (pawn.player != null)
        {
            //Debug.Log(targetPlayer);
        }

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
                pawn.sprite.flipX = false;
            }

            if (pawn.player.transform.position.x > pawn.transform.position.x)
            {
                pawn.sprite.flipX = true;
            }

            //Debug.Log(targetPlayer);
        }

        //Updates position to match with the player
        pawn.transform.position = Vector2.MoveTowards(pawn.transform.position, pawn.player.transform.position, pawnSpeed * Time.deltaTime);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}