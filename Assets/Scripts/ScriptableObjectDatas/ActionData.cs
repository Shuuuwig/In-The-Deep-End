using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionData", menuName = "ScriptableObjects / ActionData")]
public class ActionData : ScriptableObject
{
    public string ActionName;
    public AudioClip SoundClip;
    public ActionCategory ActionCategory;
    public TargetType TargetType;
    [Range(0, 10)] public int MaxActionCount;
    public StatusCategory StatusCategory;
    public float PowerMultiplier;
    [Range(0, 100)] public int Accuracy;
}
