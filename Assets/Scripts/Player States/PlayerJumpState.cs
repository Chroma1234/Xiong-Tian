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
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (player.coyoteTimeCounter > 0f)
        {
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpForce);
            player.animator.SetTrigger("jump");

            player.coyoteTimeCounter = 0f;
        }

        if (player.rb.linearVelocity.y <= 0f)
        {
            player.StateMachine.ChangeState(player.FallState);
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        player.rb.linearVelocity = new Vector2(horizontalInput * player.moveSpeed, player.rb.linearVelocity.y);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
