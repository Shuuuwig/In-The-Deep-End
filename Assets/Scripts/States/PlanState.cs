using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlanState : TurnState
{
    protected void Awake()
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter PlanState");
        LoadSelections();
    }

    protected virtual void LoadSelections()
    {
        //Reference menu handler
        //Initialize selections with personal unit data
        //Instantiate option slots or hard cap at certain number
        //Perhaps more states needed for sub-options? (player side)
    }

    protected virtual void SelectionChosen()
    {
        //Menu selection
        //Fight, Skip/Rest, Item? etc.
        //Each selection most likely hard coded to do their own thing
        //Move between states for menu navigation or handle all here
        //End after acting (unless special circumstance)

    }
}
