using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BattleData" , menuName = "ScriptableObjects / BattleData")]
public class BattleData : ScriptableObject
{
    public int RoundLength;
    public int NumberOfWaves;
    public List<GameObject> ActivePlayerUnits = new List<GameObject>(4);
    public List<GameObject> ActiveEnemyUnits = new List<GameObject>(4);

}
