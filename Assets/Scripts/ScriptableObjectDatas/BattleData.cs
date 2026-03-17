using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BattleData" , menuName = "ScriptableObjects / BattleData")]
public class BattleData : ScriptableObject
{
    public int NumberOfWaves;
    public List<GameObject> EnemyUnitsInBattle = new List<GameObject>();

}
