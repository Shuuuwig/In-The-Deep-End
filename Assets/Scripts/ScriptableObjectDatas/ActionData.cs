using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionData", menuName = "ScriptableObjects / ActionData")]
public class ActionData : ScriptableObject
{
    public string ActionName;
    public float PowerMultiplier;
    [Range(0, 3)]public int TargetableFront;
    [Range(0, 3)] public int TargetableBack;
    [Range(0, 100)] public int Accuracy;
    [Range(0, 10)] public int MaxActionCount;
    public ActionCategory ActionCategory;
    public TargetType TargetType;
    public StatusType StatusType;
}
