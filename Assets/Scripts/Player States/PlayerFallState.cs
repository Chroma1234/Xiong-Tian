using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.IdleState);
        }

        if  (Input.GetKeyDown(KeyCode.LeftShift) && player.dashCount > 0)
        {
            player.StateMachine.ChangeState(player.DashState);
            player.dashCount--;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryDoubleJump();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    private void Jump()
    {
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpForce);
        player.animator.SetTrigger("jump");
    }

    private void TryDoubleJump()
    {
        if (player.IsGrounded())
            return;

        if (!player.doubleJump)
            return;

        if (player.hasDoubleJumped)
            return;

        player.hasDoubleJumped = true;
        Jump();
    }

}
