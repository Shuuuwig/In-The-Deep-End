using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random=UnityEngine.Random;

public class EnemyPlanState : PlanState
{
    public override void Enter()
    {
        base.Enter();
        SelectionChosen();
    }

    protected override void LoadSelections()
    {
        base.LoadSelections();
        //Load moveset for enemy
    }
    protected override void SelectionChosen()
    {
        base.SelectionChosen();
        //Reactive AI, RNG or predetermined act pattern
        Debug.Log("Enemy Logic Here");
        StartCoroutine(TempTimer());
    }

    IEnumerator TempTimer() // ALL TEMPORARY
    {
        yield return new WaitForSeconds(1f);
        _currentActiveUnit.Moveset.Keys.ElementAt(0).Invoke();
        _combatHandler.SelectedAction(_currentActiveUnit.Moveset.Keys.ElementAt(0));
        int randomNumber = Random.Range(0, 2);
        _combatHandler.SaveSelectedTargets(_turnController.PlayerSpawnPos[randomNumber].GetComponentInChildren<Unit>());
        Debug.Log("Enemy Logic Ended");

        _turnController.ChangeState<ActionState>();
    }
}
