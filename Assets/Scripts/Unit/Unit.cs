using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Unit : MonoBehaviour
{
    [Header("Scriptable Object References")]
    [SerializeField] protected UnitData _unitData;
    [SerializeField] protected BattleData _battleData;
    [SerializeField] protected List<ActionData> _actionDataSO;
    protected Dictionary<UnityAction, ActionData> _moveset = new Dictionary<UnityAction, ActionData>();

    public UnitData UnitData { get { return _unitData; } }
    public Dictionary<UnityAction, ActionData> Moveset { get { return _moveset; } }

    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected List<AudioClip> _audioClips;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected List<AnimationClip> _animationClips;

    public AudioSource AudioSource { get { return _audioSource; } }
    public Animator Animator { get { return _animator; } }

    protected int _maxActionCount;
    protected float _baseTurnValue;

    public float MaxHealthPoints { get { return _unitData.MaxHealthPoints; } }
    public float MaxResolvePoints { get { return _unitData.MaxResolvePoints; } }
    public float BaseSpeed { get { return _unitData.BaseSpeed; } }
    public float BaseDamage { get { return _unitData.BaseDamage; ; } }
    public int MaxActionCount { get { return _maxActionCount; } }
    public float BaseTurnValue { get { return _baseTurnValue; } } // roundlength / speed

    [HideInInspector] public float CurrentHealthPoints;
    [HideInInspector] public float CurrentSpeed;
    [HideInInspector] public float CurrentDamage;
    [HideInInspector] public float CurrentTurnValue;
    [HideInInspector] public int CurrentActionCount;
    [HideInInspector] public AudioClip CurrentSoundClip;
    [HideInInspector] public AnimationClip DefaultIdleClip;
    [HideInInspector] public AnimationClip CurrentAnimationClip;
    [HideInInspector] public bool IsDead;

    protected virtual void Awake()
    {
        DefaultIdleClip = _animationClips[0];

        CalculateBaseTurnValue();

        CurrentTurnValue = _baseTurnValue;
        CurrentHealthPoints = MaxHealthPoints;
        CurrentSpeed = BaseSpeed;
        InitializeUnit();
        MovesetHandler();
    }

    protected virtual void Update()
    {
        // CalculateBaseTV();
    }

    protected virtual void CalculateBaseTurnValue()
    {
        _baseTurnValue = _battleData.RoundLength / BaseSpeed;
    }

    public virtual void ResetActionCount()
    {
        CurrentActionCount = 0;
    }

    public virtual void StatusCheck()
    {
        CheckHealth();
        //Check Dot effects
    }

    protected abstract void InitializeUnit();
    protected abstract void MovesetHandler();
    protected abstract void UnitUniqueUI();
    protected virtual void CheckHealth()
    {
        if (CurrentHealthPoints <= 0)
        {
            if (this == null)
                return;

            IsDead = true;
            gameObject.SetActive(false);
        }

    }
}
