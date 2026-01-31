using System.Collections;
using System.Threading;
using UnityEditor.TerrainTools;
using UnityEngine;

public class PawnIdleState : PawnState
{

    
    protected Vector3 startPos;
    protected float pawnIdleSpeed = 0.125f;

    protected bool facingRight = false;
    protected bool canMove = true;
    
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

    }  

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //raptor-x-z
        //Checking if object is allowed to move
        if (canMove)
        {
            //Moving to the left
            if (!facingRight)
            {
                pawn.transform.position = pawn.transform.position + new Vector3(this.pawn.transform.position.x * -pawnIdleSpeed * Time.deltaTime, 0, 0);
            }

            //Moving to the right
            if (facingRight)
            {
                pawn.transform.position = pawn.transform.position + new Vector3(this.pawn.transform.position.x * pawnIdleSpeed * Time.deltaTime, 0, 0);
            }
        }

        //Checking if Pawn hits the range limit
        if (pawn.transform.position.x < pawn.leftLimit.x)
        {
            canMove = false;
            Debug.Log("Left Limit reached: " + pawn.transform.position.x);

            pawn.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, 0);
            pawn.sprite.flipX = true;

            facingRight = true;
            canMove = true;

        }

        if (pawn.transform.position.x > pawn.rightLimit.x)
        {
            canMove = false;
            Debug.Log("Right Limit reached: " + pawn.transform.position.x);

            pawn.transform.position = new Vector3(pawn.transform.position.x, pawn.transform.position.y, 0);
            pawn.sprite.flipX = false;

            facingRight = false;
            canMove = true;

        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        
    }
}

