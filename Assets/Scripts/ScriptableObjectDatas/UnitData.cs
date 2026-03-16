using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects / UnitData")]
public class UnitData : ScriptableObject
{
    public float MaxHealthPoints;
    public float CurrentHealthPoints;
    public float MaxResolvePoints;
    public float CurrentResolvePoints;
    public float BaseSpeed;
    public float CurrentSpeed;
    public float BaseDamage;
    public float CurrentDamage;
    public Sprite CharacterSprite;
    public List<Sprite> CharacterIcons = new List<Sprite>();
    public List<Sprite> MarkedTargetIndicators = new List<Sprite>();
    public List<AudioClip> AudioClips;
    public List<AnimationClip> AnimationClips;
    public List<ActionData> ActionDatas = new List<ActionData>();
}
