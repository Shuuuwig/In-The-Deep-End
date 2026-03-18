using UnityEngine;

public class AllyCaptain : Unit
{
    public override void InitializeUnit()
    {
        base.InitializeUnit();

        UnitUniqueUI();
    }

    protected override void MovesetHandler()
    {
        _moveset.Clear();

        
    }

    protected override void UnitUniqueUI()
    {
        
    }

    public override void ResetActionCount()
    {
        
    }

    public override void ClearAction()
    {
        ActionUsed = null;
    }

    public override void StatusCheck()
    {
        MovesetHandler();
    }

    public override bool CanCounter()
    {
        return false;
    }

}
