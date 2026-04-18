using UnityEngine;

public class AllyFieldMedic : Unit
{
    public override void InitializeUnit()
    {
        base.InitializeUnit();

        UpdateMoveset();
    }

    protected override void UpdateMoveset()
    {
        _moveset.Clear();
        _moveset.Add(Saw, _actionDatas[0]);
        _moveset.Add(Aid, _actionDatas[1]);
        _moveset.Add(WideAid, _actionDatas[2]);
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
        throw new System.NotImplementedException();
    }

    protected void Saw()
    {
        ActionUsed = Saw;
        CurrentSoundClip = _actionDatas[0].SoundClip;

        _maxActionCount = _actionDatas[0].MaxActionCount;
        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;
    }

    protected void Aid()
    {
        ActionUsed = Aid;
        CurrentSoundClip = _actionDatas[1].SoundClip;

        _maxActionCount = _actionDatas[0].MaxActionCount;
        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;
    }

    protected void WideAid()
    {
        ActionUsed = WideAid;
        CurrentSoundClip = _actionDatas[2].SoundClip;

        _maxActionCount = _actionDatas[0].MaxActionCount;
        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;
    }
}
