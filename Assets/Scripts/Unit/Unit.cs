using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public abstract class Unit : MonoBehaviour
{
    [Header("Unit Data")]
    [SerializeField] protected UnitData _unitData;
    protected List<ActionData> _actionDatas { get { return _unitData.ActionDatas; }}
    protected Dictionary<UnityAction, ActionData> _moveset = new Dictionary<UnityAction, ActionData>();

    [Header("Component References")]
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected List<AudioClip> _audioClips;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected List<AnimationClip> _animationClips;

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
