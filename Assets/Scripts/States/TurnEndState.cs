public class TurnEndState : TurnState
{
    public override void Enter()
    {
        base.Enter();

        _combatUIHandler.ResetTargetsIndicators();

        _currentActiveUnit.ClearAction();
        _currentActiveUnit.StatusCheck();
        CombatFunctions.ClearSelectedTargets(_battleHandler.TargetedUnits);

        _battleHandler.ChangeState<TurnStartState>();
    }
}
