using System.Collections;
using System.Threading;
using UnityEngine;

public class BossIdleState : BossState
{
    protected Vector3 startPos;
    private float pawnIdleSpeed = 1f;
    private float waitTime = 1f;
    private float arriveThreshold = 0.05f;

    private Vector2 currentTarget;
    private Coroutine wanderRoutine;

    public BossIdleState(Boss boss, BossStateMachine bossStateMachine) : base(boss, bossStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();

        if (boss.isActiveAndEnabled)
        {
            currentTarget = boss.leftLimit;
            FaceTarget(currentTarget);

            wanderRoutine = boss.StartCoroutine(Wander());
        }

        boss.spriteRenderer.color = Color.white;
    }

    public override void ExitState()
    {
        if (wanderRoutine != null)
        {
            boss.StopCoroutine(wanderRoutine);
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
            while (Vector2.Distance(boss.transform.position, currentTarget) > arriveThreshold)
            {
                Vector2 targetPos = new Vector2(currentTarget.x, boss.transform.position.y);

                boss.transform.position = Vector2.MoveTowards(boss.transform.position, targetPos, pawnIdleSpeed * Time.deltaTime);
                yield return null;
            }

            // Pause at the point
            yield return new WaitForSeconds(waitTime);

            // Switch direction
            currentTarget = currentTarget == boss.leftLimit ? boss.rightLimit : boss.leftLimit;
            FaceTarget(currentTarget);
        }
    }

    private void FaceTarget(Vector2 target)
    {
        float direction = target.x - boss.transform.position.x;

        if (Mathf.Abs(direction) < 0.01f)
            return; // don't flip if basically aligned

        Vector3 scale = boss.transform.localScale;
        scale.x = direction > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        boss.transform.localScale = scale;
    }
}
