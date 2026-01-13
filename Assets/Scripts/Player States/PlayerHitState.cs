using System.Collections;
using UnityEngine;

public class PlayerHitState : PlayerState
{
    private float hitDuration = 0.25f;

    public PlayerHitState(Player player, PlayerStateMachine sm)
        : base(player, sm) { }

    public override void EnterState()
    {
        base.EnterState();
        player.animator.SetTrigger("hit");
        player.StartCoroutine(Recover());
    }

    private IEnumerator Recover()
    {
        player.StartCoroutine(player.Invincibility());
        yield return new WaitForSeconds(hitDuration);
        player.StateMachine.ChangeState(player.IdleState);
    }

    public override void FrameUpdate() { }
    public override void PhysicsUpdate() { }
}