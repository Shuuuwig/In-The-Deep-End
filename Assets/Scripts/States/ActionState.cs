using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ActionState : TurnState
{
    public override void Enter()
    {
        base.Enter();
        StartCoroutine(HandleActions());
    }

    protected override void AddListeners()
    {
        base.AddListeners();
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
    }

    protected IEnumerator HandleActions()
    {
        // 1. Get the data for the action being used
        ActionData actionDataUsed = _currentActiveUnit.Moveset[_currentActiveUnit.ActionUsed];

        if (CombatFunctions.IsEnemyTargeting(actionDataUsed.ActionCategory))
        {
            Debug.Log($"Executing Damage Action: {actionDataUsed.ActionName}");

            yield return StartCoroutine(CombatFunctions.Damage(_currentActiveUnit, _battleHandler.TargetedUnits));
        }
        else
        {
            // Handle Buffs/Heals
            //yield return StartCoroutine(CombatFunctions.InflictStatusEffect(_battleHandler.TargetedUnits, actionDataUsed.));
        }

        _battleHandler.ChangeState<TurnEndState>();
    }
}
