using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects / UnitData")]
public class UnitData : ScriptableObject
{
    public float MaxHealthPoints;
    public float MaxResolvePoints;
    public float BaseSpeed;
    public float BaseDamage;
    public Sprite CharacterSprite;
    public List<Sprite> CharacterIcons = new List<Sprite>();
    public List<Sprite> MarkedTargetIndicators = new List<Sprite>();
    public List<AudioClip> AudioClips;
    public List<AnimationClip> AnimationClips;
    public List<ActionData> ActionDatas = new List<ActionData>();
}
