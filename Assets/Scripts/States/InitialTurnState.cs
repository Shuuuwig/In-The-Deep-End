using UnityEngine;

public class InitialTurnState : TurnState
{
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Initial Turn");

        _combatUIHandler.UpdateTurnDisplay();
        _battleHandler.InitialUnitTurn();
    }
}
