using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
        Jump();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (Input.GetKeyDown(KeyCode.LeftShift) && player.dashCount > 0)
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

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        player.rb.linearVelocity = new Vector2(
            horizontalInput * player.moveSpeed,
            player.rb.linearVelocity.y
        );

        if (player.rb.linearVelocity.y <= 0f)
        {
            player.StateMachine.ChangeState(player.FallState);
        }

        //float horizontalInput = Input.GetAxisRaw("Horizontal");
        //player.rb.linearVelocity = new Vector2(horizontalInput * player.moveSpeed, player.rb.linearVelocity.y);
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
