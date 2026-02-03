using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects / UnitData")]
public class UnitData : ScriptableObject
{
    public float MaxHealthPoints;
    public float MaxResolvePoints;
    public float CurrentHealthPoints;
    public float CurrentResolvePoints;
    public float BaseSpeed;
    public float BaseDamage;
    public List<Sprite> MarkedTargetIndicatorSprites;
}
