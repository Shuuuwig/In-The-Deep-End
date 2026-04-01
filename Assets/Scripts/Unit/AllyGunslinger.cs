using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGunslingerUnit : Unit
{
    protected int _maxAmmo;
    protected int _currentAmmo;
    protected int _savedAmmo;

    [Header("Gunslinger UI")]
    protected Canvas _canvas;
    protected GameObject _passiveUI;
    protected TMP_Text _ammoNumber;

    public override void InitializeUnit()
    {
        base.InitializeUnit();
        _maxAmmo = _actionDatas[1].MaxActionCount;
        _currentAmmo = _maxAmmo;
        _savedAmmo = _currentAmmo;

        if (_canvas == null)
            _canvas = FindAnyObjectByType<Canvas>();

        UpdateMoveset();
        UnitUniqueUI();
    }

    protected override void UpdateMoveset()
    {
        if (!_moveset.ContainsKey(Reload))
        {
            _moveset.Add(Reload, _actionDatas[0]);
        }
        if (_currentAmmo > 0)
        {
            if (!_moveset.ContainsKey(Multishot))
            {
                _moveset.Add(Multishot, _actionDatas[1]);
            }
        }
        else
        {
            if (_moveset.ContainsKey(Multishot))
            {
                _moveset.Remove(Multishot);
            }
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
        _ammoNumber.color = Color.white;
        _ammoNumber.alignment = TextAlignmentOptions.BottomLeft;

        UpdateAmmoText();
    }

    public override void CheckActionCount()
    {
        AmmoCheck();
    }
    public override void ResetActionCount()
    {
        CurrentActionCount = 0;
        _currentAmmo = _savedAmmo;
        UpdateAmmoText();
        UpdateMoveset();
    }

    public override void PlanStateInitialResources()
    {
        _savedAmmo = _currentAmmo;
        _maxActionCount = _savedAmmo;
    }

    public override void ClearAction()
    {
        base.ClearAction();
    }

    public override void StatusCheck()
    {

    }

    public override bool CanCounter()
    {
        return _currentAmmo >= 2;
    }

    public override void Countered(object sender, InfoEventArgs<bool> e)
    {
        Debug.Log("Countered");
        _currentAmmo -= 2;

        UpdateAmmoText();
        UpdateMoveset();
    }

    protected void AmmoCheck()
    {
        if (ActionUsed == Reload)
        {
            _currentAmmo = _maxAmmo;
        }
        else if (ActionUsed == Multishot)
        {
            _currentAmmo--;
            _currentAmmo = Mathf.Max(0, _currentAmmo);
        }

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
        _maxActionCount = _actionDatas[0].MaxActionCount;
    }

    protected void Multishot()
    {
        ActionUsed = Multishot;

        int skillMax = _actionDatas[1].MaxActionCount;
        _maxActionCount = Mathf.Min(_currentAmmo, skillMax);

        CurrentDamage = BaseDamage * _actionDatas[1].PowerMultiplier;
    }


}