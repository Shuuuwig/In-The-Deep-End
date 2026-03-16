using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class StatusEffect : MonoBehaviour
{
    public float damagePerTick;
    public int TurnsLeft;
    public abstract IEnumerator RunEffect(Unit self);
}
