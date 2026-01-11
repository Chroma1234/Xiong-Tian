using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private float attackBufferTime = 0.15f;
    private float attackBufferCounter;

    public PlayerAttackState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
        player.animator.SetTrigger("attack");
        if (player.IsGrounded())
        {
            player.rb.linearVelocity = Vector2.zero;
        }

        player.StartCoroutine(WaitForAttackToFinish());
    }

    public override void ExitState()
    {
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

    private IEnumerator WaitForAttackToFinish()
    {
        yield return new WaitUntil(() =>
            player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        );

        yield return new WaitUntil(() =>
            !player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        );

        if (player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
        else
        {
            player.StateMachine.ChangeState(player.FallState);
        }
    }
}
