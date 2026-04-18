using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class CombatFunctions
{
    public static bool IsAllyTargeting(ActionCategory actionCategory)
    {
        switch (actionCategory)
        {
            case ActionCategory.Heal:
                return true;
        }
        return false;
    }

    public static bool IsAllyTargeting(ActionCategory actionCategory, StatusCategory statusCategory)
    {
        switch (actionCategory)
        {
            case ActionCategory.Heal:
                return true;
            case ActionCategory.StatusEffect:
                return IsPositiveStatus(statusCategory);
        }
        return false;
    }

    public static bool IsEnemyTargeting(ActionCategory actionCategory)
    {
        switch (actionCategory)
        {
            case ActionCategory.Attack:
                return true;
        }
        return false;
    }

    public static bool IsEnemyTargeting(ActionCategory actionCategory, StatusCategory statusCategory)
    {
        switch (actionCategory)
        {
            case ActionCategory.Attack:
                return true;
            case ActionCategory.StatusEffect:
                return IsPositiveStatus(statusCategory);
        }
        return false;
    }

    public static bool IsPositiveStatus(StatusCategory statusCategory)
    {
        switch (statusCategory)
        {
            case StatusCategory.Buff:
                return true;
        }
        return false;
    }

    public static bool IsBurst(TargetType targetType)
    {
        switch (targetType)
        {
            case TargetType.Burst:
                return true;
        }
        return false;
    }

    private static CombatUIHandler _combatUIHandler;

    public static void Initialize(CombatUIHandler combatUIHandler)
    {
        _combatUIHandler = combatUIHandler;
    }

    public static BattleData SelectBattleData(List<BattleData> tier1Battle, List<BattleData> tier2Battle, List<BattleData> tier3Battle, MapData mapData)
    {
        // Initialize the random state to the current time to ensure a unique seed per session
        Random.InitState((int)System.DateTime.Now.Ticks);

        Debug.Log($"Selecting Battle for Row: {mapData.CurrentRow}. Total Rows: {mapData.NumberOfRows}");

        int index = Random.Range(0, tier1Battle.Count);
            return tier1Battle[index];
    }

    public static void ResetActionCount(Unit currentActiveUnit)
    {
        currentActiveUnit.CheckActionCount();
    }

    public static List<Unit> SaveSelectedTargets(List<Unit> targetedUnits, Unit targetedUnit)
    {
        targetedUnits.Add(targetedUnit);
        Debug.Log(targetedUnits[0].name);
        return targetedUnits;
    }

    public static void ClearSelectedTargets(List<Unit> targetedUnits)
    {
        targetedUnits.Clear();
    }

    public static IEnumerator Damage(Unit self, List<Unit> targets)
    {
        for (int index = 0; index < targets.Count; index++)
        {
            if (targets[index].IsDead())
                continue;

            // 1. Play Attacker's sound (e.g., Gunshot or Sword Swing)
            if (self.CurrentSoundClip != null)
                AudioHandler.Instance.PlaySoundOneShot(self.CurrentSoundClip);

            targets[index].CurrentHealthPoints -= self.CurrentDamage;

            _combatUIHandler.UpdateHealthDisplay(targets);

            // 2. Play Target's "Hit" sound (if your Unit class has a HitSound property)
            // AudioHandler.Instance.PlaySoundOneShot(targets[index].HitSound);

            yield return new WaitForSeconds(0.2f);
        }
    }

    public static IEnumerator BurstDamage(Unit self, List<Unit> targets)
    {
        // 1. Play Attacker's burst sound once
        if (self.CurrentSoundClip != null)
            AudioHandler.Instance.PlaySoundOneShot(self.CurrentSoundClip);

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null || targets[i].IsDead()) continue;

            targets[i].CurrentHealthPoints -= self.CurrentDamage;

            // 2. Optional: Play a "Impact" sound for every individual hit
            // AudioHandler.Instance.PlaySoundOneShot(targets[i].ImpactSound);
        }

        _combatUIHandler.UpdateHealthDisplay(targets);
        yield return new WaitForSeconds(0.5f);
    }

    public static IEnumerator Heal(Unit self, List<Unit> targets)
    {
        // 1. Play the Caster's healing sound once
        if (self.CurrentSoundClip != null)
            AudioHandler.Instance.PlaySoundOneShot(self.CurrentSoundClip);

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null || targets[i].IsDead()) continue;

            float healAmount = self.CurrentDamage;
            targets[i].CurrentHealthPoints += healAmount;

            if (targets[i].CurrentHealthPoints > targets[i].MaxHealthPoints)
            {
                targets[i].CurrentHealthPoints = targets[i].MaxHealthPoints;
            }

            // 2. Optional: Individual sparkle/shimmer sound for each unit healed
            // AudioHandler.Instance.PlaySoundOneShot(targets[i].HealReceiveSound);
        }

        _combatUIHandler.UpdateHealthDisplay(targets);
        yield return new WaitForSeconds(0.5f);
    }

    public static IEnumerator Buff(Unit self, List<Unit> targets)
    {
        AudioHandler.Instance.PlaySoundOneShot(self.CurrentSoundClip);
        yield return new WaitForSeconds(0.5f);
    }

    public static void InflictStatusEffect(List<Unit> targets, StatusEffect statusEffect)
    {

    }
}
