using UnityEngine;

public class PlayerFallState : PlayerState
{
    private float dashRecoveryTimer;
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
