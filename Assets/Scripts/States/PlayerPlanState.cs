using System.Linq;
using UnityEngine;

public class PlayerPlanState : PlanState
{
    protected override void AddListeners()
    {
        base.AddListeners();
        InputHandler.NavigateMovesetEvent += OnNavigateMoveset;
        InputHandler.ConfirmActionEvent += OnConfirmAction;
        InputHandler.EarlyEndActionEvent += OnEarlyEnd;
        InputHandler.CancelActionEvent += OnCancelAction;
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        InputHandler.NavigateMovesetEvent -= OnNavigateMoveset;
        InputHandler.ConfirmActionEvent -= OnConfirmAction;
        InputHandler.EarlyEndActionEvent -= OnEarlyEnd;
        InputHandler.CancelActionEvent -= OnCancelAction;
    }

    protected void OnNavigateMoveset(object sender, InfoEventArgs<int> e)
    {
        if (_currentActiveUnit.ActionUsed == null)
            return;

        if (e.info < 0)
        {
            _combatUIHandler.CheckCurrentTarget(-1);
        }
        else if (e.info > 0)
        {
            _combatUIHandler.CheckCurrentTarget(1);
        }
    }

    protected void OnCancelAction(object sender, InfoEventArgs<bool> e)
    {
        if (e.info == true)
        {
            _currentActiveUnit.CurrentActionCount = 0;

            _currentActiveUnit.ResetActionCount();
            _currentActiveUnit.ClearAction();

            _combatUIHandler.HideCurrentIndicator();
            _combatUIHandler.ResetTargetsIndicators();

            Debug.Log("Target Canceled - Returning to Move Selection");

            LoadSelections();
        }
    }

    protected void OnConfirmAction(object sender, InfoEventArgs<bool> e)
    {
        if (_currentActiveUnit.ActionUsed == null)
            return;

        Debug.Log("Target Confirmed");
        Debug.Log($"UNIT NOW: {_currentActiveUnit}");
        _currentActiveUnit.CurrentActionCount++;
        _combatUIHandler.SaveSelectedTargets();
        Debug.Log($"Current count: {_currentActiveUnit.CurrentActionCount}, Max count: {_currentActiveUnit.MaxActionCount}");

        if (_currentActiveUnit.CurrentActionCount >= _currentActiveUnit.MaxActionCount)
        {
            Debug.Log("All targets selected. Starting action execution.");

            _combatUIHandler.HideCurrentIndicator();
            _combatUIHandler.HideSelections();
            _currentActiveUnit.CheckActionCount();

            _battleHandler.ChangeState<PlayerActionState>();
        }
        else
        {
            _currentActiveUnit.CheckActionCount();
            Debug.Log($"Target saved. {_currentActiveUnit.MaxActionCount - _currentActiveUnit.CurrentActionCount} hits remaining.");
        }
    }

    protected void OnEarlyEnd(object sender, InfoEventArgs<bool> e)
    {
        if (_currentActiveUnit.ActionUsed == null)
            return;

        if (_currentActiveUnit.CurrentActionCount < 1)
            return;

        Debug.Log("Action Ended Early - Proceeding with current targets");

        _combatUIHandler.HideCurrentIndicator();
        _combatUIHandler.HideSelections();

        _battleHandler.ChangeState<PlayerActionState>();
    }

    protected override void LoadSelections()
    {
        base.LoadSelections();
        Debug.Log("Loading Selections");

        _combatUIHandler.SetupSelections(_currentActiveUnit.Moveset.Count, _currentActiveUnit.Moveset, SelectionChosen);
    }

    protected override void SelectionChosen()
    {
        base.SelectionChosen();

        _combatUIHandler.HideSelections();
        _combatUIHandler.ShowCurrentIndicator();
    }
}
