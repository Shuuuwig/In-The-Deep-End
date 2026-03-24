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

    public static bool IsEnemyTargeting(ActionCategory actionCategory)
    {
        switch (actionCategory)
        {
            case ActionCategory.Attack:
                return true;
        }
        return false;
    }

    private static CombatUIHandler _combatUIHandler;

    public static void Initialize(CombatUIHandler combatUIHandler)
    {
        _combatUIHandler = combatUIHandler;
    }

    public static BattleData SelectBattleData(List<BattleData> tier1Battle, List<BattleData> tier2Battle, List<BattleData> tier3Battle)
    {
        if (PlayerPrefs.GetInt(MapPrefs.Row) <= 2)
            return tier1Battle[Random.Range(0, tier1Battle.Count)];

        if (PlayerPrefs.GetInt(MapPrefs.Row) == PlayerPrefs.GetInt(MapPrefs.LastRow))
            return tier3Battle[Random.Range(0, tier3Battle.Count)];
        return tier2Battle[Random.Range(0, tier2Battle.Count)];
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
            if (targets[index].IsDead()) //in case of multiple hits on same target and already died
                continue;

            targets[index].CurrentHealthPoints -= self.CurrentDamage;
            Debug.Log($"Now targeting: {targets[index]}");
            Debug.Log($"Health now: {targets[index].CurrentHealthPoints}");

            _combatUIHandler.UpdateHealthDisplay(targets);

            // AudioHandler.PlaySound(self.AudioSource, self.DamageSound);
            // AnimationHandler.PlayAnimation(self.Animator, self.AttackAnimation);

            // AudioHandler.PlaySound(targets[index].AudioSource, targets[index].TakenDamageSound);
            // AnimationHandler.PlayAnimation(targets[index].Animator, targets[index].AttackAnimation);

            yield return null;

            // float animationLength = targets[index].Animator.GetCurrentAnimatorStateInfo(0).length;

            // yield return new WaitForSeconds(animationLength);

            // AnimationHandler.PlayAnimation(self.Animator, self.DefaultIdleClip);
            // AnimationHandler.PlayAnimation(targets[index].Animator, targets[index].DefaultIdleClip);
        }
    }

    public static IEnumerator Damage(Unit self, List<StatusEffect> statusEffects)
    {
        for (int index = 0; index < statusEffects.Count; index++)
        {
            statusEffects[index].RunEffect(self);
            yield return new WaitForSeconds(1f); // change to match anim length
        }

        yield return null;
    }

    public static void InflictStatusEffect(List<Unit> targets, StatusEffect statusEffect)
    {

    }
}
