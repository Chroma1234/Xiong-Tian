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

        Debug.Log("Enters Global Attack State");

        //Teleports to the middle
        //boss.transform.position = new Vector2(boss.originalPos.x, boss.originalPos.y);


        //Activates alternating warning points, fading in + out
        //for (int i = 0; i < boss.warningList.Count; i++)
        //{

        //    if (i % 2 == 0 && !boss.triggerLeft)
        //    {
        //        GameObject stanceObject = boss.warningList[i];

        //        SpriteRenderer stanceObjRender = stanceObject.GetComponent<SpriteRenderer>();

        //        stanceObject.SetActive(true);

        //        //Coroutine to increase alpha value (visability)
        //        appearFadeIn(stanceObjRender);
        //    }

        //    if (i % 2 != 0 && boss.triggerLeft)
        //    {
        //        GameObject stanceObject = boss.warningList[i];

        //        SpriteRenderer stanceObjRender = stanceObject.GetComponent<SpriteRenderer>();

        //        stanceObject.SetActive(true);

        //        //Coroutine to increase alpha value (visability)
        //        appearFadeIn(stanceObjRender);
        //    }

        //}

        //boss.triggerLeft = !boss.triggerLeft;
        //boss.StartCoroutine(boss.triggerArrows());

        //boss.animator.SetTrigger("global");

    }

    public override void ExitState()
    {
        base.ExitState();

        Debug.Log("Exit Global Attack State");

        for (int i = 0; i < boss.warningList.Count; i++)
        {
            boss.warningList[i].SetActive(false);

        }

        for (int i = 0; i < boss.arrowList.Count; i++)
        {
            boss.arrowList[i].SetActive(false);

        }

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

    private void appearFadeIn(SpriteRenderer render)
    {
        Debug.Log("Trigger appearFadeIn()");

        //Warning pillars appear with an Alpha of 0
        Color newColor = new Color(render.color.r, render.color.g, render.color.b, 0);

        render.color = newColor;

        //Fade in warning pillars
        Debug.Log("Trigger FadeIn Coroutine");
        boss.StartCoroutine(boss.FadeTo(1f, boss.warningFadeIn, render));

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.layer == LayerMask.NameToLayer("Player") && boss.StateMachine.CurrentBossState == boss.GlobalAttackState)
        {
            boss.player = obj;
            boss.triggerGlobal = false;
            boss.exitGlobal = false;
            boss.StateMachine.ChangeState(boss.IdleState);
        }
    }

    public void OnVanishComplete()
    {
        for (int i = 0; i < boss.warningList.Count; i++)
        {
            if (i % 2 == 0 && !boss.triggerLeft)
            {
                SpriteRenderer r = boss.warningList[i].GetComponent<SpriteRenderer>();
                boss.warningList[i].SetActive(true);
                boss.StartCoroutine(boss.FadeTo(1f, boss.warningFadeIn, r));
            }
            if (i % 2 != 0 && boss.triggerLeft)
            {
                SpriteRenderer r = boss.warningList[i].GetComponent<SpriteRenderer>();
                boss.warningList[i].SetActive(true);
                boss.StartCoroutine(boss.FadeTo(1f, boss.warningFadeIn, r));
            }
        }

        boss.triggerLeft = !boss.triggerLeft;
        boss.StartCoroutine(boss.triggerArrows());
    }
}
