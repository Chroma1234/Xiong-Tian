using System.Collections;
using System.Threading;
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

        player.parryCounterAttackHitbox.SetActive(false);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!player.isTeleporting)
        {
            if (Input.GetKeyDown(KeyCode.Space) && player.coyoteTimeCounter > 0f)
            {
                player.coyoteTimeCounter = 0f;
                player.StateMachine.ChangeState(player.JumpState);
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift) && player.dashCount > 0)
            {
                player.StateMachine.ChangeState(player.DashState);
                player.dashCount--;
            }
            else if (Input.GetMouseButtonDown(1) && player.IsGrounded() && player.parryCooldownTimer <= 0f)
            {
                player.StateMachine.ChangeState(player.BlockState);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (player.hasParryCharge)
                {
                    player.StateMachine.ChangeState(player.DashState);
                    player.parryCounterAttackHitbox.SetActive(true);
                }
                else
                {
                    player.StateMachine.ChangeState(player.AttackState);
                }
            }
        }

        if (player.parryCooldownTimer > 0f)
        {
            player.parryCooldownTimer -= Time.deltaTime;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
