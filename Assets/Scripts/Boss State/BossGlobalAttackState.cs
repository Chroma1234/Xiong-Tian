using System.Collections;
using System.Threading;
using UnityEngine;

public class BossGlobalAttackState : BossState
{
    public BossGlobalAttackState(Boss boss, BossStateMachine bossStateMachine) : base(boss, bossStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();

        //Insert Global Attack Animation here
        boss.triggerGlobal = false;

        Debug.Log("Trigger Global: " + boss.triggerGlobal);

        setGlobalAttack();

        //boss.StartCoroutine(boss.exitGlobal());

    }

    public override void ExitState()
    {
        base.ExitState();

        for (int i = 0; i < boss.globalArray.Length; i++)
        {
            //boss.globalArray[i].SetActive(false);

        }

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void setGlobalAttack()
    {
        

        int spawnCount = Random.Range(1, boss.globalArray.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            if (i == 0)
            {

                //Debug.Log("Trigger setGlobalAttack");

                boss.globalArray[i].transform.position = new Vector3(boss.player.transform.position.x, boss.player.transform.position.y, 0f);

                boss.globalArray[i].SetActive(true);

                boss.StartCoroutine(boss.delayGlobalAttackTrigger(boss.globalArray[i]));

            }

            else
            {
                boss.globalArray[i].transform.position = new Vector3(boss.player.transform.position.x + Random.Range(-5f, 5f), boss.player.transform.position.y + Random.Range(-5f, 5f), 0f);

                boss.globalArray[i].SetActive(true);

                boss.StartCoroutine(boss.delayGlobalAttackTrigger(boss.globalArray[i]));
            }
        }
    }
}
