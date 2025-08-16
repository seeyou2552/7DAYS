using System;
using System.Collections.Generic;

public class AIStateMachine
{
    public AIState CurrentState { get; private set; }
    private List<Transition> transitions = new List<Transition>();
    private List<Transition> anyTransitions = new List<Transition>();

    public void SetInitialState(AIState state)
    {
        CurrentState = state;
        CurrentState.Enter();
    }

    public void AddTransition(AIState from, AIState to, Func<bool> condition)
    {
        transitions.Add(new Transition(from, to, condition));
    }

    public void AddAnyTransition(AIState to, Func<bool> condition)
    {
        anyTransitions.Add(new Transition(null, to, condition));

    }
    public void Tick()
    {

        foreach (var t in anyTransitions)
        {
            if (t.Condition())
            {
                CurrentState.Exit();
                CurrentState = t.ToState;
                CurrentState.Enter();
                return;
            }
        }

        foreach (var t in transitions)
        {
            if (t.FromState == CurrentState && t.Condition())
            {
                CurrentState.Exit();
                CurrentState = t.ToState;
                CurrentState.Enter();
                break;
            }
        }

        CurrentState?.Tick();

    }


}
