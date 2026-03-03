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

    protected IEnumerator HandleActions() // Ienumerator later
    {
        // handle damage, heal, buff and countermeasures for opposing side as well
        ActionData actionDataUsed = _currentActiveUnit.Moveset[CombatFunctions.ActionUsed];
        Debug.Log(actionDataUsed);
        if (CombatFunctions.IsDamaging(actionDataUsed.ActionType))
        {
            Debug.Log("IsDamaging");
            StartCoroutine(CombatFunctions.Damage());
            CombatFunctions.StatusCheck();
        }
        else
        {
            StartCoroutine(CombatFunctions.Buff());
        }

        yield return new WaitUntil(() => !CombatFunctions.HandlingAction);
        _turnController.ChangeState<TurnEndState>();
    }

}
