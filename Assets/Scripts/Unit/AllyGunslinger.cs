using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGunslingerUnit : Unit
{
    protected int _maxAmmo;
    protected int _currentAmmo;
    protected Dictionary<UnityAction, ActionData> _noAmmoMoveset = new Dictionary<UnityAction, ActionData>();
    protected Canvas _canvas;
    protected GameObject passiveUI;
    protected RectTransform AmmoRectTransform;
    protected TMP_Text AmmoNumber;
    protected override void InitializeUnit()
    {
        _maxAmmo = _actionDatas[0].MaxActionCount;
        _canvas = FindAnyObjectByType<Canvas>();

        CurrentActionCount = 0;
        _currentAmmo = _maxAmmo;
        UnitUniqueUI();
    }

    protected override void MovesetHandler()
    {
        _moveset.Clear();

        if (_maxAmmo - _currentAmmo >= _maxAmmo)
        {
            _moveset.Add(Reload, _actionDatas[1]);
        }
        else if (_moveset.Count == 0)
        {
            _moveset.Add(ChamberShots, _actionDatas[0]); // Swap this later
            _moveset.Add(Reload, _actionDatas[1]);
        }
    }

    protected override void UnitUniqueUI()
    {
        passiveUI = new GameObject("Gunslinger Ammo");
        passiveUI.transform.SetParent(_canvas.transform);
        passiveUI.AddComponent<TextMeshProUGUI>();

        AmmoRectTransform = passiveUI.GetComponent<RectTransform>();
        AmmoRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 60);
        AmmoRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
        AmmoRectTransform.position = new Vector3(AmmoRectTransform.position.x - 50, AmmoRectTransform.position.x - 50, AmmoRectTransform.position.x);

        AmmoNumber = passiveUI.GetComponent<TMP_Text>();
        AmmoNumber.text = $"Ammo : {_currentAmmo} / {_maxAmmo}";
        AmmoNumber.fontSize = 8f;

    }

    public override void ResetActionCount()
    {
        AmmoCheck();
        base.ResetActionCount();
    }

    public override void StatusCheck()
    {
        base.StatusCheck();
        MovesetHandler();
    }

    protected void AmmoCheck()
    {
        if (AmmoNumber == null)
            AmmoNumber = passiveUI.GetComponent<TMP_Text>();

        // if (_combatHandler.ActionUsed == Reload)
        //      _currentAmmo = _maxAmmo;
        else
            _currentAmmo -= CurrentActionCount;

        Debug.Log($"{_currentAmmo}");
        AmmoNumber.text = $"Ammo : {_currentAmmo} / {_maxAmmo}";
        AmmoNumber.fontSize = 8f;
    }

    protected void ChamberShots()
    {
        CurrentDamage = 0;

        if (_currentAmmo < _actionDatas[0].MaxActionCount)
        {
            _maxActionCount = _currentAmmo;
        }
        else
            _maxActionCount = _actionDatas[0].MaxActionCount;

        CurrentDamage = BaseDamage * _actionDatas[0].DamageMultiplier;

        CurrentSoundClip = _audioClips[0];
        CurrentAnimationClip = _animationClips[1];
    }

    protected void Reload()
    {
        CurrentDamage = 0;
        _maxActionCount = _actionDatas[1].MaxActionCount;

        CurrentSoundClip = _audioClips[1];
        CurrentAnimationClip = _animationClips[1];
    }
}
