using System.Collections;
using UnityEngine;

public class PlayerHealState : PlayerState
{
    Coroutine doHeal;
    public PlayerHealState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
        player.rb.linearVelocity = Vector2.zero;
        player.animator.SetTrigger("heal");

        doHeal = player.StartCoroutine(Heal());
    }

    public override void ExitState()
    {
        player.StopCoroutine(doHeal);
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

    private IEnumerator Heal()
    {
        yield return new WaitUntil(() =>
            player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Heal")
        );

        yield return new WaitUntil(() =>
            !player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Heal")
        );

        player.Health += player.healingAmt;
        player.Mana -= player.healingManaCost;
        player.SaveEffects();
        player.PlaySound(player.healClip);

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
