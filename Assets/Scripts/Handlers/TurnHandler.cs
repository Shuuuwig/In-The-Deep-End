// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;

// public class TurnHandler : StateMachine
// {
//     public BattleData _battleData;
//     [SerializeField] protected RoomData _roomData;
//     [SerializeField] protected RoomHandler _roomHandler;
//     [SerializeField] protected GameObject _playerGraveyard;
//     [SerializeField] protected GameObject _enemyGraveyard;
//     [SerializeField] protected List<GameObject> _playerSpawnPos = new List<GameObject>();
//     [SerializeField] protected List<GameObject> _enemySpawnPos = new List<GameObject>();

//     protected Unit _currentActiveUnit;
//     protected List<Unit> _activeUnits = new List<Unit>();

//     protected string _playerUnitTag = "PlayerUnit";
//     protected string _enemyUnitTag = "EnemyUnit";

//     public List<GameObject> PlayerSpawnPos { get { return _playerSpawnPos; } }
//     public List<GameObject> EnemySpawnPos { get { return _enemySpawnPos; } }
//     public Unit CurrentActiveUnit { get { return _currentActiveUnit; } }
//     public List<Unit> ActiveUnits { get { return _activeUnits; } }

//     protected void Awake()
//     {

//     }

//     protected void Start()
//     {
//         ChangeState<InitialGameState>();
//     }

//     public void AssignReference()
//     {
//         if (_roomHandler == null)
//             _roomHandler = FindAnyObjectByType<RoomHandler>();
//     }

//     public void InitializeActiveUnits()
//     {
//         for (int i = 0; i < _battleData.PlayerUnitsInBattle.Count; i++)
//         {
//             if (_battleData.PlayerUnitsInBattle[i] == null)
//                 break;

//             GameObject playerObject = Instantiate(_battleData.PlayerUnitsInBattle[i], _playerSpawnPos[i].transform.position, Quaternion.identity);
//             playerObject.transform.SetParent(_playerSpawnPos[i].transform);
//             playerObject.transform.position = new Vector3(_playerSpawnPos[i].transform.position.x,
//                                                           _playerSpawnPos[i].transform.position.y,
//                                                           _playerSpawnPos[i].transform.position.z + 10);

//             Unit playerUnit = playerObject.GetComponent<Unit>();
//             _activeUnits.Add(playerUnit);
//         }

//         for (int i = 0; i < _battleData.PlayerUnitsInBattle.Count; i++)
//         {
//             if (_battleData.PlayerUnitsInBattle[i] == null)
//                 break;

//             GameObject enemyObject = Instantiate(_battleData.PlayerUnitsInBattle[i], _enemySpawnPos[i].transform.position, Quaternion.identity);

//             enemyObject.transform.SetParent(_enemySpawnPos[i].transform);
//             enemyObject.transform.position = new Vector3(_enemySpawnPos[i].transform.position.x,
//                                                          _enemySpawnPos[i].transform.position.y,
//                                                          _enemySpawnPos[i].transform.position.z + 10);

//             Unit enemyUnit = enemyObject.GetComponent<Unit>();
//             _activeUnits.Add(enemyUnit);
//         }

//         Debug.Log($"Number of active units: {_activeUnits.Count}");
//         SortUnits();
//     }

//     public void SortUnits()
//     {
//         for (int i = 0; i < _activeUnits.Count; i++)
//         {
//             if (_activeUnits[i].IsDead)
//             {
//                 if (_activeUnits[i].CompareTag(_enemyUnitTag))
//                     _activeUnits[i].transform.SetParent(_enemyGraveyard.transform);

//                 if (_activeUnits[i].CompareTag(_playerUnitTag))
//                     _activeUnits[i].transform.SetParent(_playerGraveyard.transform);

//                 _activeUnits.Remove(_activeUnits[i]);
//             }
//         }

//         _activeUnits = _activeUnits.OrderBy(unit => unit.CurrentTurnValue).ToList();
//         _currentActiveUnit = _activeUnits[0];
//     }

//     public void InitialTurnValue()
//     {
//         for (int i = _activeUnits.Count - 1; i >= 0; i--)
//         {
//             _activeUnits[i].CurrentTurnValue -= _activeUnits[0].CurrentTurnValue;
//             Debug.Log($"{_activeUnits[i].gameObject.name} {i} = {_activeUnits[i].CurrentTurnValue} (Initial)");
//         }
//     }

//     public void UpdateTurnValue()
//     {
//         for (int i = 0; i < _activeUnits.Count; i++)
//         {
//             SortUnits();
//         }
//         float nextUnitTurnValue = _activeUnits[1].CurrentTurnValue;

//         if (!_activeUnits.Any(unit => unit.CompareTag(_enemyUnitTag)) || !_activeUnits.Any(unit => unit.CompareTag(_playerUnitTag)))
//         {
//             Debug.Log("all Enemy or Player are dead");
//             _roomHandler.GoToRoom(_roomData);
//             return;
//         }

//         for (int i = 0; i < _activeUnits.Count; i++)
//         {
//             if (_activeUnits[0].BaseTurnValue <= nextUnitTurnValue)
//             {
//                 if (i == 0)
//                     _activeUnits[0].CurrentTurnValue = 0;
//                 else
//                     _activeUnits[i].CurrentTurnValue -= _activeUnits[0].BaseTurnValue;
//             }
//             else
//             {
//                 _activeUnits[i].CurrentTurnValue -= nextUnitTurnValue;

//                 if (i == 0)
//                 {
//                     _activeUnits[0].CurrentTurnValue += _activeUnits[0].BaseTurnValue;
//                 }
//             }


//             Debug.Log($"{_activeUnits[i].gameObject.name} {i} = {_activeUnits[i].CurrentTurnValue}");
//         }

//         SortUnits();
//     }

//     public void DetermineTurn()
//     {
//         if (_currentActiveUnit.gameObject.CompareTag(_playerUnitTag))
//         {
//             ChangeState<PlayerPlanState>();
//             Debug.Log("Player Turn");
//         }
//         else if (_currentActiveUnit.gameObject.CompareTag(_enemyUnitTag))
//         {
//             ChangeState<EnemyPlanState>();
//             Debug.Log("Enemy Turn");
//         }
//         else
//         {
//             Debug.Log("No tags detected");
//         }
//     }
// }
