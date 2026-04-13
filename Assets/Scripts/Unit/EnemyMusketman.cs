using UnityEngine;

public class EnemyMusketman : Unit
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
        _moveset.Add(Fire, _actionDatas[0]);

    }

    protected override void UnitUniqueUI()
    {

    }

    public override void CheckActionCount()
    {

    }

    public override void ResetActionCount()
    {

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

    protected void Fire()
    {
        ActionUsed = Fire;
        _maxActionCount = _actionDatas[0].MaxActionCount;
        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;

    }
}
