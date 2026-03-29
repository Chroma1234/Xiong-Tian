using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    //private Vector2 moveDirection;
    //private float rollAccelTime = 0.08f;
    //private float rollDecelTime = 0.08f;
    Coroutine attackDelay;

    private bool inputQueued;
    private bool canQueueInput;

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
        //moveDirection = new Vector2(moveX, 0).normalized;

        player.rb.linearVelocity = Vector2.zero;
        attackDelay = player.StartCoroutine(WaitForAttackToFinish());
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (Input.GetMouseButtonDown(0) && player.IsGrounded())
        {
            if (player.comboable && canQueueInput)
            {
                player.StopCoroutine(attackDelay);
                player.animator.SetTrigger("attack");
                player.PlaySound(player.attackClip);
                attackDelay = player.StartCoroutine(WaitForAttackToFinish());
                inputQueued = false;
            }
            else
            {
                inputQueued = true;
            }
        }
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

        canQueueInput = true;

        yield return new WaitUntil(() =>
            !player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        );

        if (inputQueued && player.comboable)
        {
            player.animator.SetTrigger("attack");
            player.PlaySound(player.attackClip);
            inputQueued = false;
            canQueueInput = false;
            attackDelay = player.StartCoroutine(WaitForAttackToFinish());
            yield break;
        }

        canQueueInput = false;
        inputQueued = false;

        if (player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
        else
        {
            player.StateMachine.ChangeState(player.FallState);
        }
    }

    //private IEnumerator MoveForward()
    //{
    //    Vector2 rollDirection = moveDirection != Vector2.zero
    //        ? moveDirection
    //        : player.lastFacingDirection;

    //    Vector2 startVelocity = player.rb.linearVelocity;
    //    Vector2 maxVelocity = rollDirection * player.attackMomentum;

    //    float timer = 0f;

    //    while (timer < player.dodgeDuration)
    //    {
    //        Vector2 currentVelocity;

    //        if (timer < rollAccelTime)
    //        {
    //            float t = timer / rollAccelTime;
    //            t = Mathf.SmoothStep(0f, 1f, t);
    //            currentVelocity = Vector2.Lerp(startVelocity, maxVelocity, t);
    //        }
    //        else if (timer > player.dodgeDuration - rollDecelTime)
    //        {
    //            float t = (timer - (player.dodgeDuration - rollDecelTime)) / rollDecelTime;
    //            t = Mathf.SmoothStep(0f, 1f, t);
    //            currentVelocity = Vector2.Lerp(maxVelocity, Vector2.zero, t);
    //        }
    //        else
    //        {
    //            currentVelocity = maxVelocity;
    //        }

    //        player.rb.linearVelocity = currentVelocity;

    //        timer += Time.deltaTime;
    //        yield return null;
    //    }

    //    player.rb.linearVelocity = Vector2.zero;
    //}
}
