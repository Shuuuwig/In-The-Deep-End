using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.Events;

public class CombatUIHandler : MonoBehaviour
{
    [SerializeField] protected BattleData _battleData;
    [SerializeField] protected TurnHandler _turnHandler;
    [SerializeField] protected CombatHandler _combatHandler;
    protected ActionData _targetTypeData;

    [SerializeField] protected GameObject _actionPanel;
    [SerializeField] protected GameObject _confirmButton;
    [SerializeField] protected GameObject _buttonPrefab;
    [SerializeField] protected List<Slider> _playerHealthbars;
    [SerializeField] protected List<Slider> _enemyHealthbars;
    [SerializeField] protected List<GameObject> _turnValueDisplay;
    [SerializeField] protected List<GameObject> _movesetSelections;
    protected List<Image> _turnValuePortrait = new List<Image>();
    protected List<TMP_Text> _turnValueText = new List<TMP_Text>();
    protected List<Button> _movesetButtons = new List<Button>();
    protected List<TMP_Text> _movesetButtonText = new List<TMP_Text>();

    protected GameObject _currentTargetedPosition;
    protected Unit _currentActiveUnit { get { return _turnHandler.CurrentActiveUnit; } }
    protected SpriteRenderer _currentTargetIndicator;
    protected List<SpriteRenderer> _markedTargetIndicators = new List<SpriteRenderer>(); // this is different from current target indicator\
    [SerializeField] protected List<SpriteRenderer> _playerMarkedTargetIndicator = new List<SpriteRenderer>();
    [SerializeField] protected List<SpriteRenderer> _enemyMarkedTargetIndicator = new List<SpriteRenderer>();
    protected List<GameObject> _playerSpawnPos { get { return _turnHandler.PlayerSpawnPos; } }
    protected List<GameObject> _enemySpawnPos { get { return _turnHandler.EnemySpawnPos; } }

    public GameObject CurrentTargetedPosition { get { return _currentTargetedPosition; } }

    protected void Awake()
    {
        for (int i = 0; i < _movesetSelections.Count; i++)
        {
            _movesetButtons.Add(_movesetSelections[i].GetComponent<Button>());
            _movesetButtonText.Add(_movesetButtons[i].GetComponentInChildren<TMP_Text>());
        }

        for (int i = 0; i < _turnValueDisplay.Count; i++)
        {
            //_turnValuePortrait[i] = _turnValueDisplay[i].GetComponentInChildren<Image>();
            TMP_Text turnText = _turnValueDisplay[i].GetComponentInChildren<TMP_Text>();
            _turnValueText.Add(turnText);
        }
    }

    public void InitializeHealthDisplay()
    {
        ShowHealthbars();

        for (int i = 0; i < _playerSpawnPos.Count; i++)
        {
            for (int j = 0; j < _turnHandler.ActiveUnits.Count; j++)
            {
                if (_turnHandler.ActiveUnits[j].transform.parent.gameObject == _playerSpawnPos[i].gameObject)
                {
                    _playerHealthbars[i].maxValue = _turnHandler.ActiveUnits[j].MaxHealthPoints;
                    _playerHealthbars[i].minValue = 0;
                    _playerHealthbars[i].value = _turnHandler.ActiveUnits[j].CurrentHealthPoints;
                    Debug.Log("Initialized Player Health Display");
                }

                if (_turnHandler.ActiveUnits[j].transform.parent.gameObject == _enemySpawnPos[i].gameObject)
                {
                    _enemyHealthbars[i].maxValue = _turnHandler.ActiveUnits[j].MaxHealthPoints;
                    _enemyHealthbars[i].minValue = 0;
                    _enemyHealthbars[i].value = _turnHandler.ActiveUnits[j].CurrentHealthPoints;
                    Debug.Log("Initialized Enemy Health Display");
                }
            }

        }
    }

    public void UpdateHealthDisplay(List<Unit> targetedUnits)
    {
        for (int i = 0; i < _playerSpawnPos.Count; i++)
        {
            for (int j = 0; j < targetedUnits.Count; j++)
            {
                if (targetedUnits[j].transform.parent.gameObject == _playerSpawnPos[i].gameObject)
                {
                    _playerHealthbars[i].value = targetedUnits[j].CurrentHealthPoints;

                    Debug.Log("Updated Health Display");
                }
                else if (targetedUnits[j].transform.parent.gameObject == _enemySpawnPos[i].gameObject)
                {
                    _enemyHealthbars[i].value = targetedUnits[j].CurrentHealthPoints;

                    Debug.Log("Updated Health Display");
                }
            }
        }
        HideHealthBars();
    }

    public void ShowHealthbars()
    {
        for (int i = 0; i < _playerSpawnPos.Count; i++)
        {
            if (_playerSpawnPos[i].GetComponentInChildren<Unit>() == null)
                continue;

            bool unitDead = _playerSpawnPos[i].GetComponentInChildren<Unit>().IsDead;

            if (!unitDead)
            {
                _playerHealthbars[i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < _enemySpawnPos.Count; i++)
        {
            if (_enemySpawnPos[i].GetComponentInChildren<Unit>() == null)
                continue;

            bool unitDead = _enemySpawnPos[i].GetComponentInChildren<Unit>().IsDead;

            if (!unitDead)
            {
                _enemyHealthbars[i].gameObject.SetActive(true);
            }
        }
    }
    public void HideHealthBars()
    {
        for (int i = 0; i < _playerHealthbars.Count; i++)
        {
            if (_playerHealthbars[i].value <= 0)
                _playerHealthbars[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _enemyHealthbars.Count; i++)
        {
            if (_enemyHealthbars[i].value <= 0)
                _enemyHealthbars[i].gameObject.SetActive(false);
        }
    }

    public void SetupSelections(int movesetCount, Dictionary<UnityAction, ActionData> moveset, UnityAction acting)
    {
        _actionPanel.SetActive(true);

        for (int i = 0; i < movesetCount; i++)
        {
            if (moveset.Keys.ElementAt(i) == null) // Last left off
                continue;

            int index = i;
            _movesetSelections[i].SetActive(true);
            _movesetButtons[i].onClick.AddListener(() => _combatHandler.SelectedAction(moveset.Keys.ElementAt(index)));
            _movesetButtons[i].onClick.AddListener(moveset.Keys.ElementAt(i));
            _movesetButtons[i].onClick.AddListener(acting);
            _movesetButtonText[i].text = $"{moveset.Values.ElementAt(i).ActionName}";
        }
    }

    public void HideMovesetSelections(int movesetCount)
    {
        Debug.Log("Hiding menu");

        for (int i = 0; i < movesetCount; i++)
        {
            _movesetButtonText[i].text = $"";
            _movesetSelections[i].SetActive(false);
            _movesetButtons[i].onClick.RemoveAllListeners();
            Debug.Log("Hiding button");
        }

        _actionPanel.SetActive(false);
    }

    public void CheckCurrentTarget(int vectorDirection)
    {
        for (int i = 0; i < _enemySpawnPos.Count; i++)
        {
            if (_enemySpawnPos[i] == _currentTargetedPosition)
            {
                if (i + vectorDirection < _enemySpawnPos.Count && i + vectorDirection >= 0)
                {
                    if (_enemyHealthbars[i + vectorDirection].gameObject.activeSelf == false)
                        continue;

                    Debug.Log($"{i + vectorDirection}");
                    _currentTargetedPosition = _enemySpawnPos[i + vectorDirection];
                    _currentTargetIndicator.enabled = false;
                    ShowCurrentIndicator();
                    break;
                }
            }
            else if (_playerSpawnPos[i] == _currentTargetedPosition)
            {
                if (i + -vectorDirection < _playerSpawnPos.Count && i + -vectorDirection >= 0)
                {
                    if (_playerHealthbars[i + -vectorDirection].gameObject.activeSelf == false)
                        continue;

                    Debug.Log($"{i + vectorDirection}");
                    _currentTargetedPosition = _playerSpawnPos[i + -vectorDirection];
                    _currentTargetIndicator.enabled = false;
                    ShowCurrentIndicator();
                    break;
                }
            }
        }
    }

    public void ShowCurrentIndicator() // add parameter later for target sprite for units
    {
        if (_currentTargetedPosition == null)
            GetValidTarget();

        _currentTargetIndicator = _currentTargetedPosition.GetComponent<SpriteRenderer>();
        _currentTargetIndicator.enabled = true;

    }

    protected void GetValidTarget()
    {
        if (_combatHandler.AllyTargeting(_currentActiveUnit.Moveset[_combatHandler.ActionUsed].ActionType) == false)
        {
            for (int i = 0; i < _enemySpawnPos.Count; i++)
            {
                Unit UnitInPos = _enemySpawnPos[i].GetComponentInChildren<Unit>();
                if (UnitInPos != null)
                {
                    _currentTargetedPosition = _enemySpawnPos[i];
                    break;
                }
            }

            _markedTargetIndicators = _enemyMarkedTargetIndicator;
            Debug.Log("Not ally targeting move");
        }
        else if (_combatHandler.AllyTargeting(_currentActiveUnit.Moveset[_combatHandler.ActionUsed].ActionType))
        {
            for (int i = 0; i < _playerSpawnPos.Count; i++)
            {
                Unit UnitInPos = _playerSpawnPos[i].GetComponentInChildren<Unit>();
                if (UnitInPos != null)
                {
                    _currentTargetedPosition = _playerSpawnPos[i];
                    break;
                }
            }

            _markedTargetIndicators = _playerMarkedTargetIndicator;
            Debug.Log("Ally targeting move");
        }
        Debug.Log(_currentTargetedPosition);
    }

    public void HideCurrentIndicator()
    {
        _currentTargetIndicator.enabled = false;
        _currentTargetIndicator = null;
        _currentTargetedPosition = null;
    }

    public void SaveSelectedTargets()
    {
        for (int i = 0; i < _markedTargetIndicators.Count; i++)
        {
            if (_currentTargetedPosition == _markedTargetIndicators[i].transform.parent.gameObject)
            {
                Debug.Log("Saving Targets");
                if (_markedTargetIndicators[i].enabled)
                {
                    int spriteIndex = _currentActiveUnit.UnitData.MarkedTargetIndicators.IndexOf(_markedTargetIndicators[i].sprite);
                    _markedTargetIndicators[i].sprite = _currentActiveUnit.UnitData.MarkedTargetIndicators[spriteIndex + 1];
                }
                else
                {
                    _markedTargetIndicators[i].sprite = _currentActiveUnit.UnitData.MarkedTargetIndicators[0];
                    _markedTargetIndicators[i].enabled = true;
                }
            }
        }

    }

    public void ResetSelectedTargets()
    {
        for (int i = 0; i < _markedTargetIndicators.Count; i++)
        {
            _markedTargetIndicators[i].enabled = false;
            _markedTargetIndicators[i].sprite = null;
        }
    }

    public void UpdateTurnDisplay()
    {
        for (int i = 0; i < _turnValueText.Count; i++)
        {
            if (i < _turnHandler.ActiveUnits.Count)
            {
                _turnValueText[i].text = $"{_turnHandler.ActiveUnits[i].name} TV: {_turnHandler.ActiveUnits[i].CurrentTurnValue}";
            }
            else
            {
                _turnValueText[i].text = "";
            }
        }

    }
}
