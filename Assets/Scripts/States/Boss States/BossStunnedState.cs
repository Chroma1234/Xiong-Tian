using System.Collections;
using System.Threading;
using UnityEngine;

public class BossStunnedState : BossState
{
    private float stunDuration;
    private Coroutine stunCoroutine;

    public BossStunnedState(Boss boss, BossStateMachine bossStateMachine, float duration) : base(boss, bossStateMachine)
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

        boss.justStunned = true;
        boss.canMove = false;
        boss.Stunned();

        stunCoroutine = boss.StartCoroutine(StunTimer());
    }

    public override void ExitState()
    {
        if (stunCoroutine != null)
        {
            boss.StopCoroutine(stunCoroutine);
        }

        boss.canMove = true;
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

        boss.StateMachine.ChangeState(boss.IdleState);
    }
}
