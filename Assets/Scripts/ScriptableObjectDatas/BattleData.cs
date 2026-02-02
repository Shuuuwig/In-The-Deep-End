using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BattleData" , menuName = "ScriptableObjects / BattleData")]
public class BattleData : ScriptableObject
{
    public int RoundLength;
    public int NumberOfWaves;
    public List<GameObject> PlayerUnitsInBattle = new List<GameObject>();
    public List<GameObject> EnemyUnitsInBattle = new List<GameObject>();

}
