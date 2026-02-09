using System.Collections;
using System.Threading;
using UnityEngine;

public class PawnAttackState : PawnState
{
    private float attackCooldown = 1.0f;    // delay between attacks

    public PawnAttackState(Enemy pawn, PawnStateMachine sm) : base(pawn, sm) { }

    public override void EnterState()
    {
        pawn.canMove = false;

        TriggerAttack();
    }

    public override void ExitState()
    {
        pawn.canMove = true;
    }

    private void TriggerAttack()
    {
        pawn.animator.ResetTrigger("attack");
        pawn.animator.ResetTrigger("parryableAttack");

        //if (Random.value < 0.5f)
            //pawn.animator.SetTrigger("attack");
        //else
            pawn.animator.SetTrigger("parryableAttack");
    }

    public override void OnAttackFinished()
    {
        if (!pawn.inAttackRange)
        {
            pawn.StartCoroutine(Delay(0.5f));
            return;
        }

        pawn.StartCoroutine(AttackDelay(attackCooldown));
    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);

        pawn.StateMachine.ChangeState(pawn.ChaseState);
    }

    private IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Only trigger next attack if player still in range
        if (pawn.inAttackRange)
            TriggerAttack();
        else
            pawn.StartCoroutine(Delay(0.5f));
    }
}
