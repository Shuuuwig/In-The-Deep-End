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
        ActionData actionDataUsed = _currentActiveUnit.Moveset[_combatHandler.ActionUsed];
        Debug.Log(actionDataUsed);
        if (_combatHandler.IsDamaging(actionDataUsed.ActionType))
        {
            Debug.Log("IsDamaging");
            StartCoroutine(_combatHandler.Damage());
            _combatHandler.StatusCheck();
        }
        else
        {
            StartCoroutine(_combatHandler.Buff());
        }

        yield return new WaitUntil(() => !_combatHandler.HandlingAction);
        _turnController.ChangeState<TurnEndState>();
    }

}
