using System.Collections;
using System.Threading;
using UnityEngine;

public class BossIdleState : BossState
{
    protected Vector3 startPos;

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
            if (boss.justStunned)
            {
                boss.Teleport();
                boss.justStunned = false;
            }
            else if (boss.canMove)
            {
                currentTarget = boss.leftLimit;
                FaceTarget(currentTarget);
            }
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
