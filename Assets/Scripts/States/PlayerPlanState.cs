// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.InputSystem;

// public class PlayerPlanState : PlanState
// {
//     protected bool _actionSelected = false;
//     protected override void AddListeners()
//     {
//         base.AddListeners();
//         InputHandler.NavigateMovesetEvent += OnNavigateMoveset;
//         InputHandler.ConfirmActionEvent += OnConfirmAction;
//         InputHandler.EarlyEndActionEvent += OnEarlyEnd;
//         InputHandler.CancelActionEvent += OnCancelAction;
//     }

//     protected override void RemoveListeners()
//     {
//         base.RemoveListeners();
//         InputHandler.NavigateMovesetEvent -= OnNavigateMoveset;
//         InputHandler.ConfirmActionEvent -= OnConfirmAction;
//         InputHandler.EarlyEndActionEvent -= OnEarlyEnd;
//         InputHandler.CancelActionEvent -= OnCancelAction;
//     }

//     protected void OnNavigateMoveset(object sender, InfoEventArgs<int> e)
//     {

//         if (e.info < 0)
//         {
//             _combatUIHandler.CheckCurrentTarget(-1);
//         }
//         else if (e.info > 0)
//         {
//             _combatUIHandler.CheckCurrentTarget(1);
//         }
//     }

//     protected void OnCancelAction(object sender, InfoEventArgs<bool> e)
//     {
//         if (e.info == true)
//         {
//             if (CombatFunctions.ActionUsed == null)
//                 return;

//             _currentActiveUnit.CurrentActionCount = 0;

//             CombatFunctions.ClearAction();
//             CombatFunctions.ResetActionCount();
//             CombatFunctions.ClearSelectedTargets();

//             _combatUIHandler.HideCurrentIndicator();
//             _combatUIHandler.ResetSelectedTargets();
//             Debug.Log("Target Canceled");

//             LoadSelections();

//             _actionSelected = false;
//         }
//     }

//     protected void OnConfirmAction(object sender, InfoEventArgs<bool> e)
//     {
//         if (!_actionSelected)
//         {
//             _actionSelected = true;
//         }
//         else if (_actionSelected)
//         {
//             if (CombatFunctions.ActionUsed == null)
//                 return;

//             Debug.Log("Target Confirmed");

//             ++_currentActiveUnit.CurrentActionCount;

//             CombatFunctions.SaveSelectedTargets(_combatUIHandler.CurrentTargetedPosition.GetComponentInChildren<Unit>());
//             _combatUIHandler.SaveSelectedTargets();

//             if (_currentActiveUnit.CurrentActionCount >= _currentActiveUnit.MaxActionCount)
//             {
//                 Debug.Log("Max Action Count Reached");
//                 _actionSelected = false;

//                 _combatUIHandler.HideCurrentIndicator();
//                 _combatUIHandler.ResetSelectedTargets();
//                 CombatFunctions.ResetActionCount();

//                 _turnController.ChangeState<ActionState>();
//             }
//         }
//     }

//     protected void OnEarlyEnd(object sender, InfoEventArgs<bool> e)
//     {
//         if (_currentActiveUnit.CurrentActionCount < 1)
//             return;

//         Debug.Log("Action Ended Early");
//         _actionSelected = false;

//         _combatUIHandler.HideCurrentIndicator();
//         _combatUIHandler.ResetSelectedTargets();
//         CombatFunctions.ResetActionCount();

//         _turnController.ChangeState<ActionState>();
//     }

//     protected override void LoadSelections()
//     {
//         base.LoadSelections(); //Refer to parents class
//         Debug.Log("Loading Selections");
//         _combatUIHandler.SetupSelections(_currentActiveUnit.Moveset.Count, _currentActiveUnit.Moveset, SelectionChosen);
//     }

//     protected override void SelectionChosen()
//     {
//         base.SelectionChosen();
//         _combatUIHandler.HideMovesetSelections(_currentActiveUnit.Moveset.Count);
//         _combatUIHandler.ShowCurrentIndicator();
//     }
// }
