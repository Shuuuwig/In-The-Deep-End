using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CombatUIHandler : MonoBehaviour
{
    [Header("Input & Navigation")]
    [SerializeField] EventSystem _eventSystem;

    [Header("Core References")]
    [SerializeField] private BattleHandler _battleHandler;
    [SerializeField] private MapData _mapData;

    [Header("UI Panels & Prefabs")]
    [SerializeField] private GameObject _actionPanel;
    [SerializeField] private GameObject _counterPrompt;

    [Header("Health & Turn Displays")]
    [SerializeField] private List<TMP_Text> _unitNames;
    [SerializeField] private List<Slider> _playerHealthbars;
    [SerializeField] private List<Slider> _enemyHealthbars;
    [SerializeField] private List<GameObject> _turnValueDisplay;
    [SerializeField] private List<Image> _turnValuePortrait = new List<Image>();
    [SerializeField] private List<TMP_Text> _turnValueText = new List<TMP_Text>();

    [Header("Move Selection")]
    [SerializeField] private List<GameObject> _movesetSelections;

    [Header("Target Indicators")]
    [SerializeField] private List<SpriteRenderer> _playerMarkedTargetIndicator;
    [SerializeField] private List<SpriteRenderer> _enemyMarkedTargetIndicator;

    private ActionData _targetTypeData;
    private GameObject _currentTargetedPosition;
    private SpriteRenderer _currentTargetIndicator;

    private List<Button> _movesetButtons = new List<Button>();
    private List<TMP_Text> _movesetButtonText = new List<TMP_Text>();
    private List<SpriteRenderer> _markedTargetIndicators = new List<SpriteRenderer>();

    public GameObject CurrentTargetedPosition => _currentTargetedPosition;
    protected Unit _currentActiveUnit => _battleHandler.CurrentActiveUnit;
    protected List<GameObject> _playerSpawnPos => _battleHandler.PlayerSpawnPos;
    protected List<GameObject> _enemySpawnPos => _battleHandler.EnemySpawnPos;

    protected void Awake()
    {
        if (_eventSystem == null)
            _eventSystem = EventSystem.current;

        if (_mapData == null)
            _mapData = Resources.Load<MapData>("Maps/SavedMapData");

        foreach (GameObject selection in _movesetSelections)
        {
            Button button = selection.GetComponent<Button>();
            _movesetButtons.Add(button);
            _movesetButtonText.Add(button.GetComponentInChildren<TMP_Text>());
            Debug.Log($"sss {_movesetButtons}");
        }

        foreach (GameObject display in _turnValueDisplay)
        {
            _turnValuePortrait.Add(display.GetComponentInChildren<Image>());
            _turnValueText.Add(display.GetComponentInChildren<TMP_Text>());
        }
    }

    public void InitializeHealthDisplay()
    {
        ShowHealthbars();

        foreach (Unit unit in _battleHandler.ActiveUnits)
        {
            int index = unit.SpawnIndex;

            if (unit.IsPlayer)
            {
                if (index >= 0 && index < _playerHealthbars.Count)
                {
                    SetupHealthSlider(_playerHealthbars[index], unit);
                }
            }
            else
            {
                if (index >= 0 && index < _enemyHealthbars.Count)
                {
                    SetupHealthSlider(_enemyHealthbars[index], unit);
                }
            }
        }
    }

    private void SetupHealthSlider(Slider slider, Unit unit)
    {
        slider.maxValue = unit.MaxHealthPoints;
        slider.minValue = 0;
        slider.value = unit.CurrentHealthPoints;
    }

    public void UpdateHealthDisplay(List<Unit> targetedUnits)
    {
        foreach (Unit unit in targetedUnits)
        {
            if (unit.IsPlayer)
            {
                _playerHealthbars[unit.SpawnIndex].value = unit.CurrentHealthPoints;
            }
            else
            {
                _enemyHealthbars[unit.SpawnIndex].value = unit.CurrentHealthPoints;
            }

            Debug.Log($"Updated Health Display for {unit.name}");
        }

        HideHealthBars();
    }

    public void ShowHealthbars()
    {
        for (int i = 0; i < _battleHandler.ActiveUnits.Count; i++)
        {
            Unit unit = _battleHandler.ActiveUnits[i];

            if (unit.IsDead())
            {
                Debug.Log($"THIS UNIT {unit.name} IS DEAD");
                continue;
            }

            int uiIndex = unit.SpawnIndex;

            if (unit.IsPlayer)
            {
                if (uiIndex >= 0 && uiIndex < _playerHealthbars.Count)
                {
                    _playerHealthbars[uiIndex].gameObject.SetActive(true);
                    _unitNames[uiIndex].text = unit.name;
                }
            }
            else
            {
                if (uiIndex >= 0 && uiIndex < _enemyHealthbars.Count)
                {
                    _enemyHealthbars[uiIndex].gameObject.SetActive(true);
                }
            }
        }
    }

    public void HideHealthBars()
    {
        ToggleHealthBarList(_playerHealthbars);
        ToggleHealthBarList(_enemyHealthbars);
    }

    void ToggleHealthBarList(List<Slider> bars)
    {
        foreach (var bar in bars)
        {
            if (bar.gameObject.activeSelf && bar.value <= 0)
            {
                bar.gameObject.SetActive(false);
            }
        }
    }

    public void SetupSelections(int movesetCount, Dictionary<UnityAction, ActionData> moveset, UnityAction acting)
    {
        _actionPanel.SetActive(true);

        for (int i = 0; i < movesetCount; i++)
        {
            var movePair = moveset.ElementAtOrDefault(i);
            if (movePair.Key == null)
                continue;

            int index = i;
            UnityAction moveAction = movePair.Key;
            ActionData moveData = movePair.Value;

            _movesetSelections[index].SetActive(true);
            _movesetButtons[index].onClick.AddListener(moveAction);
            _movesetButtons[index].onClick.AddListener(acting);

            _movesetButtonText[index].text = moveData.ActionName;

            if (i == 0)
                _eventSystem.SetSelectedGameObject(_movesetSelections[i]);
        }

        PlayerPrefs.Save();
    }

    public void HideSelections()
    {
        Debug.Log("Hiding action menu");

        for (int i = 0; i < _movesetButtons.Count; i++)
        {
            _movesetButtonText[i].text = string.Empty;
            _movesetButtons[i].onClick.RemoveAllListeners();
            _movesetSelections[i].SetActive(false);
        }

        _actionPanel.SetActive(false);
    }

    public void CheckCurrentTarget(int vectorDirection)
    {
        List<GameObject> currentList;
        List<Slider> currentHealthBars;
        int step;

        if (_enemySpawnPos.Contains(_currentTargetedPosition))
        {
            currentList = _enemySpawnPos;
            currentHealthBars = _enemyHealthbars;
            step = vectorDirection;
        }
        else
        {
            currentList = _playerSpawnPos;
            currentHealthBars = _playerHealthbars;
            step = -vectorDirection;
        }

        int currentIndex = currentList.IndexOf(_currentTargetedPosition);
        int nextIndex = currentIndex + step;

        while (nextIndex >= 0 && nextIndex < currentList.Count)
        {
            if (currentHealthBars[nextIndex].gameObject.activeSelf == true)
            {
                _currentTargetedPosition = currentList[nextIndex];

                if (_currentTargetIndicator != null)
                {
                    _currentTargetIndicator.enabled = false;
                }

                ShowCurrentIndicator();
                return;
            }

            nextIndex = nextIndex + step;
        }

        Debug.Log("No valid targets found in that direction.");
    }

    public void ShowCurrentIndicator()
    {
        if (_currentTargetedPosition == null)
        {
            GetValidTarget();
            if (_currentTargetedPosition == null)
                return;
        }

        _currentTargetIndicator = _currentTargetedPosition.GetComponent<SpriteRenderer>();

        if (_currentTargetIndicator != null)
        {
            _currentTargetIndicator.enabled = true;
            Debug.Log("INDICATOR ENABLED");
        }
    }

    protected void GetValidTarget()
    {
        bool isAllyTargeting = CombatFunctions.IsAllyTargeting(_currentActiveUnit.Moveset[_currentActiveUnit.ActionUsed].ActionCategory,
        _currentActiveUnit.Moveset[_currentActiveUnit.ActionUsed].StatusCategory);

        if (isAllyTargeting == false)
        {
            for (int i = 0; i < _enemySpawnPos.Count; i++)
            {
                if (_enemyHealthbars[i].gameObject.activeSelf == true)
                {
                    _currentTargetedPosition = _enemySpawnPos[i];
                    break;
                }
            }

            _markedTargetIndicators = _enemyMarkedTargetIndicator;
            Debug.Log("Not ally targeting move - finding first living enemy");
        }
        else
        {
            for (int i = 0; i < _playerSpawnPos.Count; i++)
            {
                if (_playerHealthbars[i].gameObject.activeSelf == true)
                {
                    _currentTargetedPosition = _playerSpawnPos[i];
                    break;
                }
            }

            _markedTargetIndicators = _playerMarkedTargetIndicator;
            Debug.Log("Ally targeting move - finding first living player");
        }

        Debug.Log(_currentTargetedPosition);
    }

    public void HideCurrentIndicator()
    {
        if (_currentTargetIndicator != null)
        {
            _currentTargetIndicator.enabled = false;
        }

        _currentTargetIndicator = null;
        _currentTargetedPosition = null;
    }

    public void SaveSelectedTargets()
    {
        Unit targetedUnit = _currentTargetedPosition.GetComponentInChildren<Unit>();

        if (targetedUnit != null)
        {
            _battleHandler.TargetedUnits.Add(targetedUnit);

            int index = targetedUnit.SpawnIndex;
            SpriteRenderer markedTargetIndicator = _markedTargetIndicators[index];

            Debug.Log($"Target Saved: {targetedUnit.name}. Total Hits queued: {_battleHandler.TargetedUnits.Count}");

            if (markedTargetIndicator.enabled)
            {
                int spriteIndex = _currentActiveUnit.UnitData.MarkedTargetIndicators.IndexOf(markedTargetIndicator.sprite);

                if (spriteIndex + 1 < _currentActiveUnit.UnitData.MarkedTargetIndicators.Count)
                {
                    Debug.Log("Changing mark");
                    markedTargetIndicator.sprite = _currentActiveUnit.UnitData.MarkedTargetIndicators[spriteIndex + 1];
                }
            }
            else
            {
                markedTargetIndicator.sprite = _currentActiveUnit.UnitData.MarkedTargetIndicators[0];
                markedTargetIndicator.enabled = true;
            }
        }
    }

    public void ResetTargetsIndicators()
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
            if (i < _battleHandler.ActiveUnits.Count)
            {
                Debug.Log("Updating Turn Display");
                Unit unit = _battleHandler.ActiveUnits[i];
                _turnValueText[i].text = $"{unit.name} TV: {unit.CurrentTurnValue}";
            }
            else
            {
                _turnValueText[i].text = string.Empty;
            }
        }
    }

    public void ShowCounterPrompt()
    {
        _counterPrompt.SetActive(true);
    }

    public void HideCounterPrompt()
    {
        _counterPrompt.SetActive(false);
    }

    public void HideCounterPrompt(object sender, InfoEventArgs<bool> e)
    {
        _counterPrompt.SetActive(false);
    }
}