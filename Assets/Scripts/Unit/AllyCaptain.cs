using UnityEngine;

public class AllyCaptain : Unit
{
    public override void InitializeUnit()
    {
        base.InitializeUnit();

        UpdateMoveset();
    }

    protected override void UpdateMoveset()
    {
        _moveset.Clear();
        _moveset.Add(Strike, _actionDatas[0]);

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

    protected void Strike()
    {
        ActionUsed = Strike;
        CurrentSoundClip = _actionDatas[0].SoundClip;

        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;
        _maxActionCount = _actionDatas[0].MaxActionCount;
    }

    protected void CallToArms()
    {
        ActionUsed = CallToArms;

        CurrentDamage = 0;
    }
}
