using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CombatHandler : MonoBehaviour
{
    [HideInInspector] public bool HandlingAction;
    public UnityAction ActionUsed;
    [HideInInspector] public List<Unit> TargetedUnits;
    protected Unit _currentActingUnit { get { return _turnController.CurrentActiveUnit; } }
    [SerializeField] protected TurnController _turnController;
    [SerializeField] protected CombatUIHandler _combatUIHandler;

    public bool AllyTargeting(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Buff:
                return true;

            case ActionType.Light:
            case ActionType.Heavy:
            case ActionType.Piercing:
            case ActionType.Debuff:
                return false;

            default:
                return false;
        }
    }

    public bool IsDamaging(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Light:
            case ActionType.Heavy:
            case ActionType.Piercing:
                return true;

            case ActionType.Buff:
            case ActionType.Debuff:
                return false;

            default:
                return false;
        }
    }

    public void TargetingType(TargetType targetType)
    {
        switch (targetType)
        {
            case TargetType.Single:
                return;

            case TargetType.MultiFree:
                return;

            case TargetType.DoubleBurst:
                return;

            case TargetType.TripleBurst:
                return;

            case TargetType.QuadrupleBurst:
                return;
        }
    }

    public void ResetActionCount()
    {
        _turnController.CurrentActiveUnit.ResetActionCount();
    }

    public void StatusCheck()
    {
        for (int i = 0; i < _turnController.ActiveUnits.Count; i++)
        {
            _turnController.ActiveUnits[i].StatusCheck();
            Debug.Log($"UNITS IN LIST: {_turnController.ActiveUnits[i]}");
        }
        
        _combatUIHandler.UpdateHealthDisplay(_turnController.ActiveUnits);
        _turnController.SortUnits();

    }

    public void SelectedAction(UnityAction action)
    {
        ActionUsed = action;
    }

    public void ClearAction()
    {
        ActionUsed = null;
    }

    public void SaveSelectedTargets(Unit targetedUnit)
    {
        TargetedUnits.Add(targetedUnit);
        Debug.Log(TargetedUnits[0].name);
    }

    public void ClearSelectedTargets()
    {
        TargetedUnits.Clear();
    }

    public IEnumerator Damage() //temp (only single target)
    {
        HandlingAction = true;

        for (int i = 0; i < TargetedUnits.Count; i++)
        {
            if (TargetedUnits[i].IsDead)
                continue;

            TargetedUnits[i].CurrentHealthPoints -= _currentActingUnit.CurrentDamage;
            Debug.Log($"Now targeting: {TargetedUnits[i]}");
            Debug.Log($"Health now: {TargetedUnits[i].CurrentHealthPoints}");

            _combatUIHandler.UpdateHealthDisplay(TargetedUnits);

            AudioHandler.PlaySound(_currentActingUnit.AudioSource, _currentActingUnit.CurrentSoundClip);
            AnimationHandler.PlayAnimation(_currentActingUnit.Animator, _currentActingUnit.CurrentAnimationClip);

            yield return new WaitForSeconds(0.2f);

            AnimationHandler.PlayAnimation(_currentActingUnit.Animator, _currentActingUnit.DefaultIdleClip);
        }

        HandlingAction = false;
    }

    public IEnumerator Buff()
    {
        HandlingAction = true;

        AudioHandler.PlaySound(_currentActingUnit.AudioSource, _currentActingUnit.CurrentSoundClip);
        yield return new WaitForSeconds(0.8f);

        HandlingAction = false;
    }
}
