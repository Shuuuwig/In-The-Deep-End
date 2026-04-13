using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects / UnitData")]
public class UnitData : ScriptableObject
{
    public GameObject UnitPrefab;

    public float MaxHealthPoints;
    public float CurrentHealthPoints;

    public float MaxResolvePoints;
    public float CurrentResolvePoints;

    public float BaseSpeed;
    public float BaseDamage;

    public Sprite CharacterSprite;
    public List<Sprite> CharacterIcons = new List<Sprite>();
    public List<Sprite> MarkedTargetIndicators = new List<Sprite>();

    public List<AudioClip> AudioClips;
    public List<AnimationClip> AnimationClips;
    public List<ActionData> ActionDatas = new List<ActionData>();

    public const string SAVE_PREFIX = "Unit_";
    public const string HP_KEY = "_MaxHP";
    public const string CUR_HP_KEY = "_CurHP";
    public const string SPEED_KEY = "_Speed";
    public const string DMG_KEY = "_Dmg";

    public void SaveStatsPlayerPrefs()
    {
        // Combined Key Example: "Unit_HeroName_MaxHP"
        PlayerPrefs.SetFloat(SAVE_PREFIX + name + HP_KEY, MaxHealthPoints);
        PlayerPrefs.SetFloat(SAVE_PREFIX + name + CUR_HP_KEY, CurrentHealthPoints);
        PlayerPrefs.SetFloat(SAVE_PREFIX + name + SPEED_KEY, BaseSpeed);
        PlayerPrefs.SetFloat(SAVE_PREFIX + name + DMG_KEY, BaseDamage);

        PlayerPrefs.Save();
        Debug.Log($"Stats for {name} saved successfully.");
    }

    public void LoadStatsPlayerPrefs()
    {
        string key = SAVE_PREFIX + name + HP_KEY;

        if (PlayerPrefs.HasKey(key))
        {
            MaxHealthPoints = PlayerPrefs.GetFloat(SAVE_PREFIX + name + HP_KEY);
            CurrentHealthPoints = PlayerPrefs.GetFloat(SAVE_PREFIX + name + CUR_HP_KEY);
            BaseSpeed = PlayerPrefs.GetFloat(SAVE_PREFIX + name + SPEED_KEY);
            BaseDamage = PlayerPrefs.GetFloat(SAVE_PREFIX + name + DMG_KEY);
        }
        else
        {
            // Optional: If no save exists, ensure CurrentHP isn't 0
            ResetCurrentStats();
        }
    }

    public void ResetCurrentStats()
    {
        CurrentHealthPoints = MaxHealthPoints;
        CurrentResolvePoints = MaxResolvePoints;

        SaveStatsPlayerPrefs();
    }
}