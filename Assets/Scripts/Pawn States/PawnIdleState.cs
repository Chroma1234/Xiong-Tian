using System.Collections;
using System.Threading;
using UnityEditor.TerrainTools;
using UnityEngine;

public class PawnIdleState : PawnState
{
    protected Vector3 startPos;
    protected float pawnIdleSpeed = 1f;

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
        pawn.StartCoroutine(Wander());
    }  

    public override void ExitState()
    {
        pawn.StopAllCoroutines();
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
            if (canMove)
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
                canMove = false;
                Debug.Log("Left Limit reached: " + pawn.transform.position.x);

                yield return new WaitForSeconds(1f);

                facingRight = true;
                pawn.transform.localScale = new Vector3(-1, 1, 1);
                canMove = true;
            }

            // Right limit
            if (pawn.transform.position.x >= pawn.rightLimit.x)
            {
                canMove = false;
                Debug.Log("Right Limit reached: " + pawn.transform.position.x);

                yield return new WaitForSeconds(1f);

                facingRight = false;
                pawn.transform.localScale = Vector3.one;
                canMove = true;
            }

            yield return null;
        }
    }


}

