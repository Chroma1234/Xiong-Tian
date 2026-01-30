using UnityEngine;

public class PawnStateMachine
{
    public PawnState CurrentPawnState { get; set; }

    public void Initialize(PawnState startingState)
    {
        CurrentPawnState = startingState;
        CurrentPawnState.EnterState();
    }
    public void ChangeState(PawnState newState)
    {
        CurrentPawnState.ExitState();
        CurrentPawnState = newState;
        CurrentPawnState.EnterState();
    }
}
