using UnityEngine;

public class StartOfTurnState : TurnState
{
    public override void Enter()
    {
        base.Enter();
        _battleHandler.NextUnitTurn();
    }
}
