using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        int randomNumber = Random.Range(0, 4);

        Unit targetedUnit = _battleHandler.PlayerSpawnPos[randomNumber].transform.GetComponentInChildren<Unit>();

        if (_battleHandler.PlayerSpawnPos[randomNumber].transform.GetComponentInChildren<Unit>() == null)
        {
            if (randomNumber == 0)
                targetedUnit = _battleHandler.PlayerSpawnPos[1].transform.GetComponentInChildren<Unit>();
            else
                targetedUnit = _battleHandler.PlayerSpawnPos[0].transform.GetComponentInChildren<Unit>();
        }

        _battleHandler.TargetedUnits.Add(targetedUnit);

        Debug.Log("Enemy Logic Ended");

        _battleHandler.ChangeState<EnemyActionState>();
    }
}
