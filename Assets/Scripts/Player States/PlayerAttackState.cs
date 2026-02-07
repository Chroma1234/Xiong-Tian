using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private Vector2 moveDirection;
    private float rollAccelTime = 0.08f;
    private float rollDecelTime = 0.08f;
    Coroutine attackDelay;
    public PlayerAttackState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();
        player.animator.SetTrigger("attack");

        float moveX = Input.GetAxisRaw("Horizontal");
        moveDirection = new Vector2(moveX, 0).normalized;
        player.rb.linearVelocity += moveDirection;


        if (player.IsGrounded())
        {
            player.rb.linearVelocity = Vector2.zero;
        }

        player.StartCoroutine(MoveForward());
        attackDelay = player.StartCoroutine(WaitForAttackToFinish());
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //if(Input.GetMouseButtonDown(0) && player.IsGrounded())
        //{
        //    player.StopCoroutine(attackDelay);
        //    player.animator.SetTrigger("attack");
        //    attackDelay = player.StartCoroutine(WaitForAttackToFinish());
        //}
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private IEnumerator WaitForAttackToFinish()
    {
        yield return new WaitUntil(() =>
            player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        );

        yield return new WaitUntil(() =>
            !player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        );

        if (player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
        else
        {
            player.StateMachine.ChangeState(player.FallState);
        }
    }

    private IEnumerator MoveForward()
    {
        Vector2 rollDirection = moveDirection != Vector2.zero
            ? moveDirection
            : player.lastFacingDirection;

        Vector2 startVelocity = player.rb.linearVelocity;
        Vector2 maxVelocity = rollDirection * player.attackMomentum;

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
    }
}
