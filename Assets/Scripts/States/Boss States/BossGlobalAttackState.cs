using NUnit.Framework;
using System.Collections;
using System.Threading;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
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

        boss.animator.SetTrigger("global");
    }

    public override void ExitState()
    {
        base.ExitState();

        boss.ResetWarnings();

        boss.animator.ResetTrigger("global");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        boss.inAttackRange = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public void OnVanishComplete()
    {
        boss.warningPosX = -((boss.warningSpawnCount / 2) * boss.warningGap);

        for (int i = 0; i < boss.warningList.Count; i++)
        {
            boss.warningList[i].gameObject.transform.position = new Vector2(boss.originalPos.x + boss.warningPosX, boss.originalPos.y);
            boss.warningPosX += boss.warningGap;

            if (i % 2 == 0 && !boss.triggerLeft)
            {
                SpriteRenderer r = boss.warningList[i].GetComponent<SpriteRenderer>();

                GameObject stanceWarningObj = boss.warningList[i].gameObject.transform.GetChild(0).gameObject;

                SpriteRenderer r2 = stanceWarningObj.GetComponent<SpriteRenderer>();

                boss.warningList[i].SetActive(true);

                boss.StartCoroutine(boss.FadeTo(1f, boss.warningFadeIn, r));
                boss.StartCoroutine(boss.FadeTo(1f, boss.warningFadeIn, r2));
            }

            if (i % 2 != 0 && boss.triggerLeft)
            {
                SpriteRenderer r = boss.warningList[i].GetComponent<SpriteRenderer>();

                GameObject stanceWarningObj = boss.warningList[i].gameObject.transform.GetChild(0).gameObject;

                SpriteRenderer r2 = stanceWarningObj.GetComponent<SpriteRenderer>();

                boss.warningList[i].SetActive(true);

                boss.StartCoroutine(boss.FadeTo(1f, boss.warningFadeIn, r));
                boss.StartCoroutine(boss.FadeTo(1f, boss.warningFadeIn, r2));
            }
        }

        if (boss.warningGap > boss.minGapValue)
        {
            boss.warningGap -= boss.decreaseValue;
        }

        boss.triggerLeft = !boss.triggerLeft;
        boss.StartCoroutine(boss.triggerArrows());
    }
}
