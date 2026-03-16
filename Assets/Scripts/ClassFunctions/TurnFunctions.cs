using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class TurnFunctions
{
    public static List<Unit> SortActiveUnits(List<Unit> activeUnits, string playerUnitTag, string enemyUnitTag,
                                             GameObject playerGraveyard, GameObject enemyGraveyard)
    {
        for (int index = 0; index < activeUnits.Count; index++)
        {
            if (activeUnits[index].IsDead())
            {
                if (activeUnits[index].CompareTag(enemyUnitTag))
                    activeUnits[index].transform.SetParent(enemyGraveyard.transform);

                if (activeUnits[index].CompareTag(playerUnitTag))
                    activeUnits[index].transform.SetParent(playerGraveyard.transform);

                activeUnits.Remove(activeUnits[index]);
            }
        }
        return activeUnits = activeUnits.OrderBy(unit => unit.CurrentTurnValue).ToList();
    }

    public static Unit CurrentActiveUnit(List<Unit> activeUnits)
    {
        return activeUnits[0];
    }

    public static void InitialTurnValue(List<Unit> activeUnits, string playerUnitTag, string enemyUnitTag,
                                     GameObject playerGraveyard, GameObject enemyGraveyard)
    {
        foreach (Unit unit in activeUnits)
        {
            unit.CurrentTurnValue = unit.BaseTurnValue;
        }

        SortActiveUnits(activeUnits, playerUnitTag, enemyUnitTag, playerGraveyard, enemyGraveyard);

        float fastestUnitValue = activeUnits[0].CurrentTurnValue;

        for (int index = 0; index < activeUnits.Count; index++)
        {
            activeUnits[index].CurrentTurnValue -= fastestUnitValue;

            Debug.Log($"{activeUnits[index].gameObject.name} {index} = {activeUnits[index].CurrentTurnValue} (Initial)");
        }
    }

    public static void UpdateTurnValue(List<Unit> activeUnits, string playerUnitTag, string enemyUnitTag,
                                    GameObject playerGraveyard, GameObject enemyGraveyard, RoomData mapRoom)
    {
        SortActiveUnits(activeUnits, playerUnitTag, enemyUnitTag, playerGraveyard, enemyGraveyard);

        bool playerAlive = false;
        bool enemyAlive = false;

        foreach (Unit unit in activeUnits)
        {
            if (unit.CompareTag(playerUnitTag))
                playerAlive = true;
            if (unit.CompareTag(enemyUnitTag))
                enemyAlive = true;
        }

        if (!playerAlive || !enemyAlive)
        {
            // change to victory ui later
            RoomHandler.GoToRoom(mapRoom);
            return;
        }

        float turnValuePassed = activeUnits[0].CurrentTurnValue;

        for (int index = 0; index < activeUnits.Count; index++)
        {
            if (index == 0)
            {
                activeUnits[index].CurrentTurnValue = activeUnits[index].BaseTurnValue;
            }
            else
            {
                activeUnits[index].CurrentTurnValue -= turnValuePassed;

                if (activeUnits[index].CurrentTurnValue < 0)
                {
                    activeUnits[index].CurrentTurnValue = 0;
                }
            }

            Debug.Log($"{activeUnits[index].gameObject.name} {index} = {activeUnits[index].CurrentTurnValue}");
        }

        SortActiveUnits(activeUnits, playerUnitTag, enemyUnitTag, playerGraveyard, enemyGraveyard);
    }

    public static TurnType DetermineTurn(List<Unit> activeUnits, string playerUnitTag, string enemyUnitTag)
    {
        if (CurrentActiveUnit(activeUnits).CompareTag(playerUnitTag))
            return TurnType.PlayerTurn;
        if (CurrentActiveUnit(activeUnits).CompareTag(enemyUnitTag))
            return TurnType.EnemyTurn;

        return TurnType.None;
    }
}
