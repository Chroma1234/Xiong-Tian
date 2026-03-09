using System.Collections;
using System.Threading;
using System.Xml;
using UnityEngine;

public class BossChaseState : BossState
{
    protected Vector2 targetPlayer;
    protected float pawnSpeed = 2.0f;

    private float groundCheckDistance = 0.2f;
    private LayerMask groundLayer = LayerMask.GetMask("Ground");

    private Coroutine startG;

    public BossChaseState(Boss boss, BossStateMachine bossStateMachine) : base(boss, bossStateMachine)
    {
    }

    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log("Entered Chased State");

        //Trigger the coroutine to trigger global attacks after a set amount of time
        if (startG == null)
        {
            startG = boss.StartCoroutine(boss.GlobalTimer());
        }

        else
        {
            boss.StopCoroutine(startG);
            startG = null;

            startG = boss.StartCoroutine(boss.GlobalTimer());

        }

    }
    
    public override void ExitState()
    {
        base.ExitState();
        Debug.Log("Exit Chased State");

        if (startG != null)
        {
            boss.StopCoroutine(startG);
            startG = null;

            Debug.Log("Disabled coroutine");
        }
        
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //raptor-x-z
        if (boss.player != null)
        {
            //Gets player position
            targetPlayer = boss.player.transform.position;

            if (boss.player.transform.position.x < boss.transform.position.x)
            {
                boss.transform.localScale = Vector3.one;
            }

            if (boss.player.transform.position.x > boss.transform.position.x)
            {
                boss.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        if (boss.inAttackRange)
        {
            boss.StateMachine.ChangeState(boss.AttackState);
        }

        //else if (pawn.canMove)
        //{
        //Updates position to match with the player
        if (IsGroundAhead())
        {
            boss.transform.position = Vector2.MoveTowards(boss.transform.position, new Vector3(boss.player.transform.position.x, boss.transform.position.y, boss.transform.position.z), pawnSpeed * Time.deltaTime);

            //Bool condition to enter Global Attack State == true
            if (boss.StateMachine.CurrentBossState != boss.DeadState && boss.StateMachine.CurrentBossState != boss.AttackState && boss.StateMachine.CurrentBossState != boss.StunnedState && boss.StateMachine.CurrentBossState != boss.GlobalAttackState && boss.triggerGlobal)          
            {
                //Debug.Log("Trigger Global: " + boss.triggerGlobal);         

                boss.StateMachine.ChangeState(boss.GlobalAttackState);
                boss.StopCoroutine(boss.GlobalTimer());

                //Global State Cooldown
                boss.StartCoroutine(boss.GlobalStateCooldown());
            }
        }
        else if (!IsGroundAhead())
        {
            boss.StateMachine.ChangeState(boss.IdleState);
        }
        //}   

        ////Checking if Pawn hits the range limit
        //if (pawn.transform.position.x < pawn.leftLimit.x + 0.5)
        //{
        //    pawn.StateMachine.ChangeState(pawn.IdleState);
        //}

        //if (pawn.transform.position.x > pawn.rightLimit.x - 0.5)
        //{
        //    pawn.StateMachine.ChangeState(pawn.IdleState);
        //}


        //if (boss.StateMachine.CurrentBossState != boss.DeadState && boss.StateMachine.CurrentBossState == boss.AttackState || boss.StateMachine.CurrentBossState == boss.StunnedState || boss.StateMachine.CurrentBossState == boss.DeadState && boss.triggerGlobal && startG != null)
        //{
        //    boss.StopCoroutine(startG);
        //    startG = null;

        //    Debug.Log("Disabled coroutine");
        //}
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    bool IsGroundAhead()
    {
        return Physics2D.Raycast(boss.groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }
}
