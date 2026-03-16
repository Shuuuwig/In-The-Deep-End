using System.Collections;
using System.Collections.Generic;

public class TurnState : State
{
    protected BattleHandler _battleHandler;
    protected CombatUIHandler _combatUIHandler;
    protected Unit _currentActiveUnit => _battleHandler.CurrentActiveUnit;

    public override void Enter()
    {
        base.Enter();
    }

    public virtual void SetHandlers(BattleHandler battleHandler, CombatUIHandler combatUIHandler)
    {
        _battleHandler = battleHandler;
        _combatUIHandler = combatUIHandler;
    }

    protected virtual IEnumerator CheckStatus()
    {
        yield return null;
    }

}
