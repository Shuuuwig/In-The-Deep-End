using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleHandler : StateMachine
{
    [Header("Room Data")]
    [SerializeField] protected RoomData _currentRoomData;
    [SerializeField] protected MapData _mapData;
    [SerializeField] protected TeamData _teamData;
    [SerializeField] private AudioClip _battleMusic;
    [SerializeField] protected List<BattleData> _tier1Battle = new List<BattleData>();
    [SerializeField] protected List<BattleData> _tier2Battle = new List<BattleData>();
    [SerializeField] protected List<BattleData> _tier3Battle = new List<BattleData>();
    protected BattleData _battleData;

    [Header("Scene References")]
    [SerializeField] protected CombatUIHandler _combatUIHandler;
    [SerializeField] protected GameObject _playerGraveyard;
    [SerializeField] protected GameObject _enemyGraveyard;
    [SerializeField] protected List<GameObject> _playerSpawnPos = new List<GameObject>();
    [SerializeField] protected List<GameObject> _enemySpawnPos = new List<GameObject>();

    [Header("Battle State")]
    protected Unit _currentActiveUnit;
    protected List<Unit> _activeUnits = new List<Unit>();
    protected HashSet<GameObject> _unactiveUnitObjects = new HashSet<GameObject>();
    protected List<Unit> _targetedUnits = new List<Unit>();
    protected TurnType _turnType;

    [Header("Settings & Tags")]
    public const float TurnTVLength = 150f;
    protected string _playerUnitTag = "PlayerUnit";
    protected string _enemyUnitTag = "EnemyUnit";

    public MapData MapRoomData => _mapData;
    public string PlayerUnitTag => _playerUnitTag;
    public string EnemyUnitTag => _enemyUnitTag;
    public GameObject PlayerGraveyard => _playerGraveyard;
    public GameObject EnemyGraveyard => _enemyGraveyard;
    public List<Unit> TargetedUnits => _targetedUnits;
    public List<GameObject> PlayerSpawnPos => _playerSpawnPos;
    public List<GameObject> EnemySpawnPos => _enemySpawnPos;
    public Unit CurrentActiveUnit => _currentActiveUnit;
    public List<Unit> ActiveUnits => _activeUnits;

    void Start()
    {
        AudioHandler.Instance.PlayMusic(_battleMusic, true);
        InitializeBattle();
        CombatFunctions.Initialize(_combatUIHandler);
    }

    public override void ChangeState<T>()
    {
        _newState = GetComponent<T>();

        if (_newState == null)
            _newState = gameObject.AddComponent<T>();

        if (_newState is TurnState turnState)
            turnState.SetHandlers(this, _combatUIHandler);

        TransitionState(_newState);
    }

    public void InitializeBattle()
    {
        _battleData = CombatFunctions.SelectBattleData(_tier1Battle, _tier2Battle, _tier3Battle, _mapData);
        _teamData = Resources.Load<TeamData>("TeamData/PlayerTeamData");

        for (int i = 0; i < _teamData.UnitsInParty.Count; i++)
        {
            if (_teamData.UnitsInParty[i] == null)
            {
                break;
            }

            GameObject playerObject = Instantiate(_teamData.UnitsInParty[i], _playerSpawnPos[i].transform);

            playerObject.name = _teamData.UnitsInParty[i].name;
            playerObject.transform.localPosition = new Vector3(0, 0, 10);

            Unit playerUnit = playerObject.GetComponent<Unit>();
            playerUnit.InitializeUnit();

            playerUnit.SpawnIndex = i;
            playerUnit.IsPlayer = true;

            _activeUnits.Add(playerUnit);
        }

        for (int i = 0; i < _battleData.EnemyUnitsInBattle.Count; i++)
        {
            if (_battleData.EnemyUnitsInBattle[i] == null)
            {
                break;
            }

            GameObject enemyObject = Instantiate(_battleData.EnemyUnitsInBattle[i], _enemySpawnPos[i].transform);

            enemyObject.name = _battleData.EnemyUnitsInBattle[i].name;
            enemyObject.transform.localPosition = new Vector3(0, 0, 10);

            Unit enemyUnit = enemyObject.GetComponent<Unit>();
            enemyUnit.InitializeUnit();

            enemyUnit.SpawnIndex = i;
            enemyUnit.IsPlayer = false;

            _activeUnits.Add(enemyUnit);
        }

        Debug.Log($"Number of active units: {_activeUnits.Count}");

        _activeUnits = TurnFunctions.SortActiveUnits(_activeUnits);
        _currentActiveUnit = TurnFunctions.CurrentActiveUnit(_activeUnits);
        TurnFunctions.InitialTurnValue(_activeUnits, _playerUnitTag, _enemyUnitTag, _playerGraveyard, _enemyGraveyard);
        _activeUnits = TurnFunctions.SortActiveUnits(_activeUnits);

        _combatUIHandler.InitializeHealthDisplay();
        _combatUIHandler.InitializeUnitDetails();

        ChangeState<InitialTurnState>();
    }

    protected void UnitTurn()
    {
        TurnType turnType = TurnFunctions.DetermineTurn(_activeUnits, _playerUnitTag, _enemyUnitTag);
        _currentActiveUnit = TurnFunctions.CurrentActiveUnit(_activeUnits);
        _currentActiveUnit.StatusCheck();

        if (turnType == TurnType.PlayerTurn)
        {
            Debug.Log($"Player turn: {_currentActiveUnit}");
            ChangeState<PlayerPlanState>();
        }
        else if (turnType == TurnType.EnemyTurn)
        {
            Debug.Log($"Enemy turn: {_currentActiveUnit}");
            ChangeState<EnemyPlanState>();
        }
        else
            Debug.Log("Noones turn");
    }

    public void InitialUnitTurn()
    {
        UnitTurn();
    }

    public void NextUnitTurn()
    {
        TurnFunctions.UpdateTurnValue(_activeUnits, _playerUnitTag, _enemyUnitTag, _mapData, _teamData);
        _activeUnits = TurnFunctions.SortActiveUnits(_activeUnits);
        UnitTurn();
    }

    public void CheckForDead()
    {
        for (int index = _activeUnits.Count - 1; index >= 0; index--)
        {
            if (_activeUnits[index].IsDead())
            {
                Unit deadUnit = _activeUnits[index];

                if (deadUnit.IsPlayer)
                    deadUnit.transform.SetParent(_playerGraveyard.transform);
                else
                {
                    deadUnit.transform.SetParent(_enemyGraveyard.transform);
                    Debug.Log($"ADDED {deadUnit.name} to graveyard");
                }

                _activeUnits.RemoveAt(index);
            }
        }
    }

    public void Checkgraveyard()
    {
        Debug.Log("Checking Graveyard");
        foreach (Transform child in _playerGraveyard.transform)
        {
            _unactiveUnitObjects.Add(child.gameObject);
        }

        foreach (Transform child in _enemyGraveyard.transform)
        {
            _unactiveUnitObjects.Add(child.gameObject);
        }

        foreach (GameObject unitObjects in _unactiveUnitObjects)
        {
            Debug.Log($"HIDING {unitObjects}");
            unitObjects.SetActive(false);
        }
    }
}
