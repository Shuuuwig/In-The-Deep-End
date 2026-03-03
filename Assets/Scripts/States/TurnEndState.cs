using System;
using System.Collections;
using UnityEngine;

public class TurnEndState : TurnState
{
    public override void Enter()
    {
        base.Enter();

        _combatUIHandler.ResetSelectedTargets();

        CombatFunctions.ClearAction();
        CombatFunctions.StatusCheck();
        CombatFunctions.ClearSelectedTargets();

        TurnTransition();
    }
}
