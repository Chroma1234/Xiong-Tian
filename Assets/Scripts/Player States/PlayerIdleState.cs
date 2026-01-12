using System.Threading;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private float dashRecoveryTimer;

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
        float timer = 0f;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (player.dashCount < 3)
        {
            dashRecoveryTimer += Time.deltaTime;

            if (dashRecoveryTimer >= player.dashRecoveryTime)
            {
                player.dashCount++;
                dashRecoveryTimer = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateMachine.ChangeState(player.JumpState);
        }
        else if(Input.GetKeyDown(KeyCode.LeftShift) && player.dashCount > 0)
        {
            player.StateMachine.ChangeState(player.DashState);
            player.dashCount--;
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
