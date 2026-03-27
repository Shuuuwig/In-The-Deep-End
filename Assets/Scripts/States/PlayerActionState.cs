using System.Collections;
using UnityEngine;

public class PlayerActionState : ActionState
{
    protected override IEnumerator HandleActions()
    {
        ActionData actionDataUsed = _currentActiveUnit.Moveset[_currentActiveUnit.ActionUsed];

        if (_battleHandler.TargetedUnits[0].CanCounter())
        {
            Debug.Log("Can Counter");
            yield return new WaitForSeconds(1f);
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
