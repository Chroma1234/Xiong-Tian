using System.Collections;
using System.Threading;
using UnityEngine;

public class PawnIdleState : PawnState
{
    protected Vector3 startPos;
    private float pawnIdleSpeed = 1f;
    private float waitTime = 1f;
    private float arriveThreshold = 0.05f;

    private Vector2 currentTarget;
    private Coroutine wanderRoutine;

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

        if (pawn.isActiveAndEnabled)
        {
            currentTarget = pawn.leftLimit;
            FaceTarget(currentTarget);

            wanderRoutine = pawn.StartCoroutine(Wander());
        }
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

    private IEnumerator Wander()
    {
        while (true)
        {
            // Move toward current target
            while (Vector2.Distance(pawn.transform.position, currentTarget) > arriveThreshold)
            {
                Vector2 targetPos = new Vector2(currentTarget.x, pawn.transform.position.y);

                pawn.transform.position = Vector2.MoveTowards(pawn.transform.position, targetPos, pawnIdleSpeed * Time.deltaTime);
                yield return null;
            }

            // Pause at the point
            yield return new WaitForSeconds(waitTime);

            // Switch direction
            currentTarget = currentTarget == pawn.leftLimit ? pawn.rightLimit : pawn.leftLimit;
            FaceTarget(currentTarget);
        }
    }

    private void FaceTarget(Vector2 target)
    {
        float direction = target.x - pawn.transform.position.x;

        if (Mathf.Abs(direction) < 0.01f)
            return; // don't flip if basically aligned

        Vector3 scale = pawn.transform.localScale;
        scale.x = direction > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        pawn.transform.localScale = scale;
    }
}

