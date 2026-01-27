using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TeamData", menuName = "ScriptableObjects / TeamData")]
public class TeamData
{
    public List<GameObject> selectedUnits = new List<GameObject>(4);
}
