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

        Debug.Log("Enters Global Attack State");

        //Teleports to the middle
        boss.transform.position = new Vector2(boss.originalPos.x, boss.originalPos.y);


        //Activates alternating warning pointS
        for (int i = 0; i < boss.warningList.Count; i++)
        {


            if (i % 2 == 0 && boss.globalEven)
            {
                boss.warningList[i].SetActive(true);
            }

            if (i % 2 != 0 && !boss.globalEven)
            {
                boss.warningList[i].SetActive(true);
            }

        }

        boss.globalEven = !boss.globalEven;


        //Coroutine to trigger damageble warning points


        //


    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log("Exit Global Attack State");

        for (int i = 0; i < boss.warningList.Count; i++)
        {
            boss.warningList[i].SetActive(false);   
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
}
