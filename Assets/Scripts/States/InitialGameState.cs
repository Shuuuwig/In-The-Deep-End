using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InitialGameState : TurnState
{
    public override void Enter()
    {
        base.Enter();
        TurnTransition();
    }

    protected override void TurnTransition()
    {

    }
}
