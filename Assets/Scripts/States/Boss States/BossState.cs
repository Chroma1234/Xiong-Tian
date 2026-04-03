using UnityEngine;

public class BossState
{
    protected Boss boss;
    protected BossStateMachine bossStateMachine;

    public BossState(Boss boss, BossStateMachine bossStateMachine)
    {
        this.boss = boss;
        this.bossStateMachine = bossStateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTriggerEvent() { }
    public virtual void OnAttackFinished() { }

}
