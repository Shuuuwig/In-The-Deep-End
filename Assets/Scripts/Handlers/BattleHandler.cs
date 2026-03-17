using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : StateMachine
{
    [Header("Battle Setup Data")]
    [SerializeField] protected RoomData _roomData;
    protected TeamData _teamData;
    [SerializeField] protected List<BattleData> Tier1Battle = new List<BattleData>();
    [SerializeField] protected List<BattleData> Tier2Battle = new List<BattleData>();
    [SerializeField] protected List<BattleData> Tier3Battle = new List<BattleData>();
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
    protected List<Unit> _targetedUnits = new List<Unit>();
    protected TurnType _turnType;

    [Header("Settings & Tags")]
    public const float TurnTVLength = 150f;
    protected string _playerUnitTag = "PlayerUnit";
    protected string _enemyUnitTag = "EnemyUnit";

    public List<Unit> TargetedUnits => _targetedUnits;
    public List<GameObject> PlayerSpawnPos => _playerSpawnPos;
    public List<GameObject> EnemySpawnPos => _enemySpawnPos;
    public Unit CurrentActiveUnit => _currentActiveUnit;
    public List<Unit> ActiveUnits => _activeUnits;

    void Start()
    {
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
        _battleData = CombatFunctions.SelectBattleData(Tier1Battle, Tier2Battle, Tier3Battle);
        _teamData = Resources.Load<TeamData>("TeamData/PlayerTeamData");

        for (int i = 0; i < _teamData.UnitsInParty.Count; i++)
        {
            Unit playerUnit = _teamData.UnitsInParty[i].GetComponent<Unit>();

            if (_teamData.UnitsInParty[i] == null)
                break;

            if (playerUnit.IsDead())
            {
                Debug.Log($"{playerUnit.name} is dead with {playerUnit.CurrentHealthPoints} health");
                continue;
            }


            GameObject playerObject = Instantiate(_teamData.UnitsInParty[i], _playerSpawnPos[i].transform);

            playerObject.transform.localPosition = new Vector3(0, 0, 10);

            playerUnit.SpawnIndex = i;
            playerUnit.IsPlayer = true;

            _activeUnits.Add(playerUnit);
        }

        for (int i = 0; i < _battleData.EnemyUnitsInBattle.Count; i++)
        {
            if (_battleData.EnemyUnitsInBattle[i] == null)
                break;

            GameObject enemyObject = Instantiate(_battleData.EnemyUnitsInBattle[i], _enemySpawnPos[i].transform);
            enemyObject.transform.localPosition = new Vector3(0, 0, 10);

            Unit enemyUnit = enemyObject.GetComponent<Unit>();

            enemyUnit.SpawnIndex = i;
            enemyUnit.IsPlayer = false;

            _activeUnits.Add(enemyUnit);
        }

        Debug.Log($"Number of active units: {_activeUnits.Count}");

        _activeUnits = TurnFunctions.SortActiveUnits(_activeUnits, _playerUnitTag, _enemyUnitTag, _playerGraveyard, _enemyGraveyard);
        _currentActiveUnit = TurnFunctions.CurrentActiveUnit(_activeUnits);
        TurnFunctions.InitialTurnValue(_activeUnits, _playerUnitTag, _enemyUnitTag, _playerGraveyard, _enemyGraveyard);

        _combatUIHandler.InitializeHealthDisplay();

        ChangeState<StartOfTurnState>();
    }

    public void NextUnitTurn()
    {
        TurnType turnType = TurnFunctions.DetermineTurn(_activeUnits, _playerUnitTag, _enemyUnitTag);
        _currentActiveUnit = TurnFunctions.CurrentActiveUnit(_activeUnits);
        _currentActiveUnit.StatusCheck();

        if (turnType == TurnType.PlayerTurn)
            ChangeState<PlayerPlanState>();
        if (turnType == TurnType.EnemyTurn)
            ChangeState<PlayerPlanState>();
        else
            Debug.Log("Noones turn");
    }
}
