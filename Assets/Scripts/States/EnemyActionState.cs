using System.Collections;
using UnityEngine;

public class EnemyActionState : ActionState
{
    public override void Enter()
    {
        StartCoroutine(HandleActions());
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        InputHandler.CounterActionEvent += _combatUIHandler.HideCounterPrompt;
        InputHandler.CounterActionEvent += _battleHandler.TargetedUnits[0].Countered;
        InputHandler.CounterActionEvent += CounterConfirmed;
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        InputHandler.CounterActionEvent -= _combatUIHandler.HideCounterPrompt;
        InputHandler.CounterActionEvent -= _battleHandler.TargetedUnits[0].Countered;
        InputHandler.CounterActionEvent -= CounterConfirmed;
    }

    protected override IEnumerator HandleActions()
    {
        ActionData actionDataUsed = _currentActiveUnit.Moveset[_currentActiveUnit.ActionUsed];
        // loop this later
        if (_battleHandler.TargetedUnits[0].CanCounter())
        {
            Debug.Log($"{_battleHandler.TargetedUnits[0]} CAN COUNTER");
            _combatUIHandler.ShowCounterPrompt();
            AddListeners();
            yield return new WaitForSeconds(1.5f);
            _combatUIHandler.HideCounterPrompt();
            RemoveListeners();
        }
        else if (CombatFunctions.IsEnemyTargeting(actionDataUsed.ActionCategory))
        {
            Debug.Log($"Executing Damage Action: {actionDataUsed.ActionName}");

            yield return StartCoroutine(CombatFunctions.Damage(_currentActiveUnit, _battleHandler.TargetedUnits));
        }
        else
        {
            // Handle Buffs/Heals
            //yield return StartCoroutine(CombatFunctions.InflictStatusEffect(_battleHandler.TargetedUnits, actionDataUsed.));
        }

        _battleHandler.ChangeState<EndOfTurnState>();
    }
}
