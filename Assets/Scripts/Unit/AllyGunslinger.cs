using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGunslingerUnit : Unit
{
    protected int _maxAmmo;
    protected int _currentAmmo;

    [Header("Gunslinger UI")]
    protected Canvas _canvas;
    protected GameObject _passiveUI;
    protected TMP_Text _ammoNumber;

    public override void InitializeUnit()
    {
        base.InitializeUnit();

        _maxAmmo = _actionDatas[0].MaxActionCount;
        _currentAmmo = _maxAmmo;

        if (_canvas == null) _canvas = FindAnyObjectByType<Canvas>();

        UnitUniqueUI();
    }

    protected override void MovesetHandler()
    {
        _moveset.Clear();

        _moveset.Add(Reload, _actionDatas[0]);
        
        if (_currentAmmo > 0)
        {
            _moveset.Add(Multishot, _actionDatas[1]);
        }


    }

    protected override void UnitUniqueUI()
    {
        if (_passiveUI == null)
        {
            _passiveUI = new GameObject("Gunslinger Ammo");
            _passiveUI.transform.SetParent(_canvas.transform, false);
            _ammoNumber = _passiveUI.AddComponent<TextMeshProUGUI>();
        }

        RectTransform rect = _passiveUI.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);

        rect.anchoredPosition = new Vector2(50, 50);
        rect.sizeDelta = new Vector2(200, 50);

        _ammoNumber.fontSize = 24f;
        _ammoNumber.alignment = TextAlignmentOptions.BottomLeft;

        UpdateAmmoText();
    }

    public override void ResetActionCount()
    {
        AmmoCheck();
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
        return _currentAmmo >= 2;
    }

    protected void AmmoCheck()
    {
        if (ActionUsed == Reload)
        {
            _currentAmmo = _maxAmmo;
        }
        else if (ActionUsed == Multishot)
        {
            _currentAmmo -= CurrentActionCount;
        }

        _currentAmmo = Mathf.Clamp(_currentAmmo, 0, _maxAmmo);
        UpdateAmmoText();
    }

    private void UpdateAmmoText()
    {
        if (_ammoNumber != null)
        {
            _ammoNumber.text = $"Ammo : {_currentAmmo} / {_maxAmmo}";
        }
    }

    protected void Reload()
    {
        ActionUsed = Reload;

        CurrentDamage = 0;
        _maxActionCount = _actionDatas[1].MaxActionCount;

        //DamageSound = _audioClips[1];
        //AttackAnimation = _animationClips[1];
    }

    protected void Multishot()
    {
        ActionUsed = Multishot;

        if (_currentAmmo < _actionDatas[0].MaxActionCount)
        {
            _maxActionCount = _currentAmmo;
        }
        else
        {
            _maxActionCount = _actionDatas[0].MaxActionCount;
        }

        CurrentDamage = BaseDamage * _actionDatas[0].PowerMultiplier;
        //DamageSound = _audioClips[0];
        //AttackAnimation = _animationClips[1];
    }


}