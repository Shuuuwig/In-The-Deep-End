using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Unit : MonoBehaviour
{
    [Header("Unit Data & Moveset")]
    [SerializeField] protected UnitData _unitData;
    protected Dictionary<UnityAction, ActionData> _moveset = new Dictionary<UnityAction, ActionData>();

    public UnityAction ActionUsed { get; protected set; }
    public UnitData UnitData => _unitData;
    public Dictionary<UnityAction, ActionData> Moveset => _moveset;
    protected List<ActionData> _actionDatas => _unitData.ActionDatas;

    [Header("Component References")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Animator _animator;
    public AudioSource AudioSource => _audioSource;
    public Animator Animator => _animator;

    [Header("Battle Identity")]
    public int SpawnIndex;
    public bool IsPlayer;
    public List<StatusEffect> _statusEffects = new List<StatusEffect>();

    [Header("Growth Stats (Live Maxes)")]
    public int CurrentLevel = 1;
    public float MaxHealthPoints; // No longer just a pointer to _unitData
    public float MaxResolvePoints;
    public float BaseSpeed;
    public float BaseDamage;

    [Header("Live Battle State")]
    [HideInInspector] public float CurrentHealthPoints;
    [HideInInspector] public float CurrentSpeed;
    [HideInInspector] public float CurrentDamage;
    [HideInInspector] public float CurrentTurnValue;
    [HideInInspector] public int CurrentActionCount;

    [Header("Cached Assets")]
    [HideInInspector] public AnimationClip DefaultIdleClip;
    [HideInInspector] public AnimationClip AttackAnimation;
    [HideInInspector] public AnimationClip TakenDamage;
    [HideInInspector] public AudioClip DamageSound;
    [HideInInspector] public AudioClip TakenDamageSound;

    protected int _maxActionCount;
    protected float _baseTurnValue;
    public float BaseTurnValue => _baseTurnValue;
    public int MaxActionCount => _maxActionCount;

    protected List<AudioClip> _audioClips => _unitData.AudioClips;
    protected List<AnimationClip> _animationClips => _unitData.AnimationClips;

    public virtual bool IsDead() => CurrentHealthPoints <= 0;

    protected virtual void Awake()
    {
        if (_animationClips != null && _animationClips.Count > 0)
            DefaultIdleClip = _animationClips[0];

        InitializeUnit();
        MovesetHandler();
    }

    protected virtual void Update() { }

    public virtual void ResetActionCount()
    {
        CurrentActionCount = 0;
    }

    public virtual void ClearAction()
    {
        ActionUsed = null;
    }

    protected virtual void InitializeUnit()
    {
        MaxHealthPoints = _unitData.MaxHealthPoints;
        MaxResolvePoints = _unitData.MaxResolvePoints;
        BaseSpeed = _unitData.BaseSpeed;
        BaseDamage = _unitData.BaseDamage;

        CurrentHealthPoints = MaxHealthPoints;
        CurrentSpeed = BaseSpeed;
        CurrentDamage = BaseDamage;

        _baseTurnValue = BattleHandler.TurnTVLength / BaseSpeed;
        CurrentTurnValue = _baseTurnValue;
    }

    public virtual void LevelUp(float hpGain, float dmgGain, float speedGain)
    {
        CurrentLevel++;

        MaxHealthPoints += hpGain;
        BaseDamage += dmgGain;
        BaseSpeed += speedGain;

        CurrentHealthPoints = MaxHealthPoints;

        Debug.Log($"{gameObject.name} leveled up to {CurrentLevel}!");
    }

    

    protected abstract void MovesetHandler();
    protected abstract void UnitUniqueUI();
    public abstract void StatusCheck();
    public abstract bool CanCounter();
}