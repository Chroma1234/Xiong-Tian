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

        if (boss.justStunned)
        {
            boss.Teleport();
            boss.justStunned = false;
        }
    }
    
    public override void ExitState()
    {
        base.ExitState();

        if (startG != null)
        {
            boss.StopCoroutine(startG);
            startG = null;
        }
        
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (boss.canMove)
        {
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

            if (boss.inAttackRange && !boss.justStunned && !boss.justTeleported)
            {
                boss.StateMachine.ChangeState(boss.AttackState);
            }

            //Updates position to match with the player
            if (IsGroundAhead())
            {
                boss.transform.position = Vector2.MoveTowards(boss.transform.position, new Vector3(boss.player.transform.position.x, boss.transform.position.y, boss.transform.position.z), pawnSpeed * Time.deltaTime);

                //Bool condition to enter Global Attack State == true
                if (boss.StateMachine.CurrentBossState != boss.DeadState && boss.StateMachine.CurrentBossState != boss.StunnedState && boss.StateMachine.CurrentBossState != boss.GlobalAttackState && boss.triggerGlobal)
                {      
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
        }
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
