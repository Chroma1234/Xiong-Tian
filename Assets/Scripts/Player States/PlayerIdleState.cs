using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateMachine.ChangeState(player.JumpState);
        }
        else if(Input.GetKeyDown(KeyCode.LeftShift) && player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.DashState);
        }
        else if (Input.GetMouseButtonDown(1) && player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.BlockState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
