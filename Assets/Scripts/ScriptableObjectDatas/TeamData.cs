using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TeamData", menuName = "ScriptableObjects / TeamData")]
public class TeamData : ScriptableObject
{
    public List<GameObject> UnitsInParty = new List<GameObject>();
}
