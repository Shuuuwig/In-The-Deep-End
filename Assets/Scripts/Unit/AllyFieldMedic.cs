using UnityEngine;

public class AllyFieldMedic : Unit
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
        _moveset.Add(Saw, _actionDatas[0]);

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
        throw new System.NotImplementedException();
    }

    protected void Saw()
    {
        ActionUsed = Saw;
        _maxActionCount = _actionDatas[0].MaxActionCount;
        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;
    }

    protected void Aid()
    {
        ActionUsed = Aid;
    }
}
