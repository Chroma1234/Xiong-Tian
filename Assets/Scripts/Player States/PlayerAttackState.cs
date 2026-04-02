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
    private bool freezeHorizontal;

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

        freezeHorizontal = true; // freeze movement during attack
        //moveAttack();

        //float moveX = Input.GetAxisRaw("Horizontal");
        //moveDirection = new Vector2(moveX, 0).normalized;
        //player.rb.linearVelocity = Vector2.zero;

        attackDelay = player.StartCoroutine(WaitForAttackToFinish());
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (Input.GetMouseButtonDown(0) && player.IsGrounded() && canQueueInput)
        {

                // Always queue input (buffer it)
                inputQueued = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // Stop horizontal sliding
        if (freezeHorizontal)
        {
            // Stop horizontal sliding during the attack
            player.rb.linearVelocity = new Vector2(0f, player.rb.linearVelocity.y);
        }
    }

    private IEnumerator WaitForAttackToFinish()
    {
        // Wait until attack animation actually starts
        yield return new WaitUntil(() =>
            player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        );

        // Wait until early part of animation (open combo window earlier)
        yield return new WaitUntil(() =>
        {
            var state = player.animator.GetCurrentAnimatorStateInfo(0);
            return state.IsTag("Attack") && state.normalizedTime >= 0.2f;
        });

        // Open combo window
        canQueueInput = true;

        // Keep window open for a short, consistent duration
        float timer = 0f;
        float comboWindow = 1f;

        while (timer < comboWindow)
        {
            timer += Time.deltaTime;

            if (inputQueued && player.comboable)
            {
                yield return new WaitForSeconds(0.075f); // small delay
                // Trigger next attack immediately
                player.animator.SetTrigger("attack");
                player.PlaySound(player.attackClip);

                inputQueued = false;
                canQueueInput = false;

                attackDelay = player.StartCoroutine(WaitForAttackToFinish());
                yield break;
            }

            // Check if animation finished
            var stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsTag("Attack") || stateInfo.normalizedTime >= 1f)
            {
                break; // exit attack loop
            }

            yield return null;
        }

        // Close window
        canQueueInput = false;
        inputQueued = false;
        freezeHorizontal = false;

        /*yield return new WaitUntil(() =>
        {
            var state = player.animator.GetCurrentAnimatorStateInfo(0);
            return !state.IsTag("Attack") || state.normalizedTime >= 1f;
        });*/


        if (player.IsGrounded())
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
        else
        {
            player.StateMachine.ChangeState(player.FallState);
        }
        yield break;
    }


    private void moveAttack()
    {
        if (player.transform.localScale == Vector3.one)
        {
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x + player.attackDisplacement, player.rb.linearVelocity.y);
        }

        else
        {
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x - player.attackDisplacement, player.rb.linearVelocity.y);
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
