using UnityEngine;

public class EndOfTurnState : TurnState
{
    public override void Enter()
    {
        base.Enter();
        Debug.Log("End of Turn");
        _battleHandler.CheckForDead();
        _battleHandler.Checkgraveyard();
        _combatUIHandler.ResetTargetsIndicators();

        _currentActiveUnit.ResetActionCount();
        _currentActiveUnit.ClearAction();
        _currentActiveUnit.StatusCheck();
        CombatFunctions.ClearSelectedTargets(_battleHandler.TargetedUnits);

        _battleHandler.ChangeState<StartOfTurnState>();
    }
}
