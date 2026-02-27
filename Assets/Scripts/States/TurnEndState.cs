using System;
using System.Collections;
using UnityEngine;

public class TurnEndState : TurnState
{
    public override void Enter()
    {
        base.Enter();

        _combatUIHandler.ResetSelectedTargets();

        _combatHandler.ClearAction();
        _combatHandler.StatusCheck();
        _combatHandler.ClearSelectedTargets();

        TurnTransition();
    }
}
