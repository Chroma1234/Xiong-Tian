using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private Vector2 moveDirection;
    private float rollAccelTime = 0.08f;
    private float rollDecelTime = 0.08f;
    public PlayerDashState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();

        player.DisablePlayerCollider();

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        player.StartCoroutine(Dash());
    }

    public override void ExitState()
    {
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

    private IEnumerator Dash()
    {
        Vector2 rollDirection = moveDirection != Vector2.zero
            ? moveDirection
            : player.lastFacingDirection;

        Vector2 startVelocity = player.rb.linearVelocity;
        Vector2 maxVelocity = rollDirection * player.dodgeSpeed;

        float timer = 0f;

        while (timer < player.dodgeDuration)
        {
            Vector2 currentVelocity;

            if (timer < rollAccelTime)
            {
                float t = timer / rollAccelTime;
                t = Mathf.SmoothStep(0f, 1f, t);
                currentVelocity = Vector2.Lerp(startVelocity, maxVelocity, t);
            }
            else if (timer > player.dodgeDuration - rollDecelTime)
            {
                float t = (timer - (player.dodgeDuration - rollDecelTime)) / rollDecelTime;
                t = Mathf.SmoothStep(0f, 1f, t);
                currentVelocity = Vector2.Lerp(maxVelocity, Vector2.zero, t);
            }
            else
            {
                currentVelocity = maxVelocity;
            }

            player.rb.linearVelocity = currentVelocity;

            timer += Time.deltaTime;
            yield return null;
        }

        player.rb.linearVelocity = Vector2.zero;
        player.EnablePlayerCollider();
        player.StateMachine.ChangeState(player.IdleState);
    }

}
