using System.Collections;
using System.Threading;
using UnityEngine;

public class BossAttackState : BossState
{
    private float attackCooldown =1.0f;    // delay between attacks

    public BossAttackState(Boss boss, BossStateMachine sm) : base(boss, sm) { }

    public override void EnterState()
    {
        boss.canMove = false;

        TriggerAttack();

        Debug.Log("Enter Attack State");

    }

    public override void ExitState()
    {
        boss.canMove = true;
        Debug.Log("Exit Attack State");
    }

    private void TriggerAttack()
    {
        Debug.Log("TriggerAttack()");
        Debug.Log(boss.inAttackRange);

        boss.animator.ResetTrigger("attack");
        boss.animator.ResetTrigger("parryableAttack");

        //if (Random.value < 0.5f)
        //pawn.animator.SetTrigger("attack");
        //else
        boss.animator.SetTrigger("parryableAttack");
    }

    public override void OnAttackFinished()
    {
        Debug.Log("onAttackFinished()");

        if (!boss.inAttackRange)
        {
            boss.StartCoroutine(Delay(0.5f));
            return;
        }

        boss.StartCoroutine(AttackDelay(attackCooldown));
    }

    private IEnumerator Delay(float delay)
    {
        Debug.Log("Delay()");

        yield return new WaitForSeconds(delay);

        boss.StateMachine.ChangeState(boss.ChaseState);
    }

    private IEnumerator AttackDelay(float delay)
    {
        Debug.Log("AttackDelay()");

        yield return new WaitForSeconds(delay);

        // Only trigger next attack if player still in range
        if (boss.inAttackRange)
            TriggerAttack();
        else
            boss.StartCoroutine(Delay(0.5f));
    }
}
