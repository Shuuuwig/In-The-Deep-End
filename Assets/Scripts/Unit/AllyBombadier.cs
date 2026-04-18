using UnityEngine;

public class AllyBombadier : Unit
{
    public override void InitializeUnit()
    {
        base.InitializeUnit();

        UpdateMoveset();
    }

    protected override void UpdateMoveset()
    {
        _moveset.Clear();
        _moveset.Add(Bombardment, _actionDatas[0]);
    }

    public override void UnitDetails(GameObject detailsPosition)
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

    public override void PlanStateResetActionCount()
    {
        CurrentActionCount = 0;
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

    protected void Bombardment()
    {
        ActionUsed = Bombardment;
        CurrentSoundClip = _actionDatas[0].SoundClip;

        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;
        _maxActionCount = _actionDatas[0].MaxActionCount;
    }
}
