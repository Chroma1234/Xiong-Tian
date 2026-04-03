using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
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

        freezeHorizontal = true;


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
            inputQueued = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (freezeHorizontal)
        {
            player.rb.linearVelocity = new Vector2(0f, player.rb.linearVelocity.y);
        }
    }

    private IEnumerator WaitForAttackToFinish()
    {
        yield return new WaitUntil(() =>
            player.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        );

        yield return new WaitUntil(() =>
        {
            var state = player.animator.GetCurrentAnimatorStateInfo(0);
            return state.IsTag("Attack") && state.normalizedTime >= 0.2f;
        });

        canQueueInput = true;

        float timer = 0f;
        float comboWindow = 1f;

        while (timer < comboWindow)
        {
            timer += Time.deltaTime;

            if (inputQueued && player.comboable)
            {
                yield return new WaitForSeconds(0.075f);

                player.animator.SetTrigger("attack");

                inputQueued = false;
                canQueueInput = false;

                attackDelay = player.StartCoroutine(WaitForAttackToFinish());
                yield break;
            }

            // Check if animation finished
            var stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsTag("Attack") || stateInfo.normalizedTime >= 1f)
            {
                break;
            }

            yield return null;
        }

        canQueueInput = false;
        inputQueued = false;
        freezeHorizontal = false;

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
}
