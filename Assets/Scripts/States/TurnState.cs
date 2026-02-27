using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TurnState : State
{
    protected TurnHandler _turnController;
    protected CombatHandler _combatHandler;
    protected CombatUIHandler _combatUIHandler;
    protected List<Unit> _activeUnits { get { return _turnController.ActiveUnits; } }
    protected Unit _currentActiveUnit { get { return _turnController.CurrentActiveUnit; } }

    public override void Enter()
    {
        base.Enter();
        InitializeReferences();
    }

    protected virtual void InitializeReferences()
    {
        if (_turnController == null)
            _turnController = FindAnyObjectByType<TurnHandler>();

        if (_combatHandler == null)
            _combatHandler = FindAnyObjectByType<CombatHandler>();

        if (_combatUIHandler == null)
            _combatUIHandler = FindAnyObjectByType<CombatUIHandler>();
    }

    protected virtual void TurnTransition()
    {
        _turnController.UpdateTurnValue();
        _turnController.DetermineTurn();
        _combatUIHandler.UpdateTurnDisplay();
    }
}
