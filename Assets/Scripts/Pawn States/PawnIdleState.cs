using System.Collections;
using System.Threading;
using UnityEngine;

public class PawnIdleState : PawnState
{
    protected Vector3 startPos;
    protected float pawnIdleSpeed = 1f;

    protected bool facingRight = false;

    Coroutine wanderRoutine;
    
    public PawnIdleState(Enemy pawn, PawnStateMachine pawnStateMachine) : base(pawn, pawnStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("im idle");

        startPos = pawn.transform.position;
        wanderRoutine = pawn.StartCoroutine(Wander());
    }  

    public override void ExitState()
    {
        if (wanderRoutine != null)
        {
            pawn.StopCoroutine(wanderRoutine);
        }
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

    public IEnumerator Wander()
    {
        while (true)
        {
            if (pawn.canMove)
            {
                if (!facingRight)
                {
                    pawn.transform.localScale = Vector3.one;
                    pawn.transform.position += Vector3.left * pawnIdleSpeed * Time.deltaTime;
                }
                else
                {
                    pawn.transform.localScale = new Vector3(-1, 1, 1);
                    pawn.transform.position += Vector3.right * pawnIdleSpeed * Time.deltaTime;
                }
            }

            // Left limit
            if (pawn.transform.position.x <= pawn.leftLimit.x)
            {
                pawn.canMove = false;

                yield return new WaitForSeconds(1f);

                facingRight = true;
                pawn.transform.localScale = new Vector3(-1, 1, 1);
                pawn.canMove = true;
            }

            // Right limit
            if (pawn.transform.position.x >= pawn.rightLimit.x)
            {
                pawn.canMove = false;

                yield return new WaitForSeconds(1f);

                facingRight = false;
                pawn.transform.localScale = Vector3.one;
                pawn.canMove = true;
            }

            yield return null;
        }
    }


}

