using UnityEngine;

public class PlayerCastingState : PlayerState
{
    private float castTimer;
    private bool soulFired;

    public PlayerCastingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();

        Soul yangSoul = player.soulController.equippedYangSoul;

        if (yangSoul == null || player.Mana < yangSoul.manaCost)
        {
            player.StateMachine.ChangeState(player.IdleState);
            return;
        }

        player.rb.linearVelocity = Vector2.zero;
        soulFired = false;
        castTimer = 0f;

        player.animator.SetTrigger("cast");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        castTimer += Time.deltaTime;

        if (!soulFired && castTimer >= 0.3f)
        {
            FireSoul();
        }

        if (castTimer >= 0.5f)
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void FireSoul()
    {
        Soul yangSoul = player.soulController.equippedYangSoul;

        soulFired = true;
        player.Mana -= yangSoul.manaCost;
        yangSoul.OnActivate(player);
    }
}