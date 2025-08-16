using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class AIState
{

    protected GameObject owner;

    public AIState(GameObject owner)
    {
        this.owner = owner;
    }


    public abstract void Enter();
    public abstract void Tick();
    public abstract void Exit();
}
