using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects / MapData")]
public class MapData : ScriptableObject
{
    public int MapSeed;
    public int NumberOfRows;
    public int CurrentRoom;
    public int CurrentRow;
}
