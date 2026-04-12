using UnityEngine;

public class EnemyCaptain : Unit
{
    public override void InitializeUnit()
    {
        base.InitializeUnit();

        UpdateMoveset();
        UnitUniqueUI();
    }

    protected override void UpdateMoveset()
    {
        _moveset.Add(Slash, _actionDatas[0]);
        //_moveset.Add(AllHandsOnDeck, _actionDatas[1]);
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

    protected void Slash()
    {
        ActionUsed = Slash;
        _maxActionCount = _actionDatas[0].MaxActionCount;
        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;
    }

    protected void AllHandsOnDeck()
    {

    }
}
