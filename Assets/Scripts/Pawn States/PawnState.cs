using UnityEngine;

public class PawnState
{
    protected Enemy pawn;
    protected PawnStateMachine pawnStateMachine;

    public PawnState(Enemy pawn, PawnStateMachine pawnStateMachine)
    {
        this.pawn = pawn;
        this.pawnStateMachine = pawnStateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTriggerEvent() { }
    public virtual void OnAttackFinished() { }
}
