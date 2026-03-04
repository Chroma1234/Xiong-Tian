using System.Collections;
using UnityEngine;

public class PlayerBlockState : PlayerState
{
    public PlayerBlockState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();

        player.parryCooldownTimer = player.parryCooldown;

        player.rb.linearVelocity = Vector2.zero;
        player.EnableBlockCollider();
        player.parryRingFX.SetActive(true);
        player.parryRingFX.GetComponent<ParticleSystem>().Play();

        player.StartCoroutine(ParryWindow(player.parryDuration));
    }

    public override void ExitState()
    {
        base.ExitState();

        player.parryRingFX.GetComponent<ParticleSystem>().Stop();
        player.parryRingFX.SetActive(false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private IEnumerator ParryWindow(float time)
    {
        yield return new WaitForSeconds(time);

        player.DisableBlockCollider();
        player.StateMachine.ChangeState(player.IdleState);
    }
}
