using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Transition
{
    public AIState FromState { get; }
    public AIState ToState { get; }
    public Func<bool> Condition { get; }

    public Transition(AIState from, AIState to, Func<bool> condition)
    {
        FromState = from;
        ToState = to;
        Condition = condition;
    }
}