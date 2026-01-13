using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    Coroutine attackDelay;
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

        attackDelay = player.StartCoroutine(WaitForAttackToFinish());
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if(Input.GetMouseButtonDown(0) && player.IsGrounded())
        {
            player.StopCoroutine(attackDelay);
            player.animator.SetTrigger("attack");
            attackDelay = player.StartCoroutine(WaitForAttackToFinish());
        }
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
