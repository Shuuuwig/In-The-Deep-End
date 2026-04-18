using System.Collections;
using UnityEngine;

public class PlayerActionState : ActionState
{
    protected override IEnumerator HandleActions()
    {
        ActionData actionDataUsed = _currentActiveUnit.Moveset[_currentActiveUnit.ActionUsed];

        // 1. Counter Check (Usually checks the primary/first target)
        if (_battleHandler.TargetedUnits.Count > 0 && _battleHandler.TargetedUnits[0].CanCounter())
        {
            Debug.Log("Can Counter");
            yield return new WaitForSeconds(1f);
        }
        // 2. Check if this is an offensive action
        else if (CombatFunctions.IsEnemyTargeting(actionDataUsed.ActionCategory))
        {
            Debug.Log($"Executing Damage Action: {actionDataUsed.ActionName}");

            // --- BURST CHECK START ---
            if (CombatFunctions.IsBurst(actionDataUsed.TargetType))
            {
                yield return StartCoroutine(CombatFunctions.BurstDamage(_currentActiveUnit, _battleHandler.TargetedUnits));
            }
            else
            {
                yield return StartCoroutine(CombatFunctions.Damage(_currentActiveUnit, _battleHandler.TargetedUnits));
            }
        }
        else if (actionDataUsed.ActionCategory == ActionCategory.Heal)
        {
            yield return StartCoroutine(CombatFunctions.Heal(_currentActiveUnit, _battleHandler.TargetedUnits));
        }
        else if (actionDataUsed.ActionCategory == ActionCategory.StatusEffect)
        {
            yield return StartCoroutine(CombatFunctions.Buff(_currentActiveUnit, _battleHandler.TargetedUnits));
        }

        _battleHandler.ChangeState<EndOfTurnState>();
    }
}
