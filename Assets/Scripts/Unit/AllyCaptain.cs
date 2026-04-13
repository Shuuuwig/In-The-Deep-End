using UnityEngine;

public class AllyCaptain : Unit
{
    public override void InitializeUnit()
    {
        base.InitializeUnit();

        UpdateMoveset();
        UnitUniqueUI();
    }

    protected override void UpdateMoveset()
    {
        _moveset.Clear();
        _moveset.Add(Strike, _actionDatas[0]);

    }

    protected override void UnitUniqueUI()
    {

    }

    public override void CheckActionCount()
    {

    }

    public override void ResetActionCount()
    {
        CurrentActionCount = 0;
    }

    public override void PlanStateInitialResources()
    {

    }

    public override void ClearAction()
    {
        ActionUsed = null;
    }

    public override void StatusCheck()
    {

    }

    public override bool CanCounter()
    {
        return false;
    }

    public override void Countered(object sender, InfoEventArgs<bool> e)
    {

    }

    protected void Strike()
    {
        ActionUsed = Strike;

        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;
        _maxActionCount = _actionDatas[0].MaxActionCount;
    }

    protected void CallToArms()
    {
        ActionUsed = CallToArms;

        CurrentDamage = 0;
    }
}
