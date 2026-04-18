using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(fileName = "TeamData", menuName = "ScriptableObjects / TeamData")]
public class TeamData : ScriptableObject
{
    // 0 - Front, 1 - Forward Centre, 2 - Back Centre, 3 - Back
    public List<GameObject> UnitsInParty = new List<GameObject>() { null, null, null, null };

    private const string TEAM_KEY_PREFIX = "TeamSlot_";

    public void SaveTeam()
    {
        for (int i = 0; i < UnitsInParty.Count; i++)
        {
            if (UnitsInParty[i] != null)
            {
                PlayerPrefs.SetString(TEAM_KEY_PREFIX + i, UnitsInParty[i].name);
            }
            else
            {
                PlayerPrefs.SetString(TEAM_KEY_PREFIX + i, "");
            }
        }
        PlayerPrefs.Save();
        Debug.Log("TeamData: Team composition saved to disk.");

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

    public void LoadTeam()
    {
        for (int i = 0; i < UnitsInParty.Count; i++)
        {
            string unitName = PlayerPrefs.GetString(TEAM_KEY_PREFIX + i, "");

            if (!string.IsNullOrEmpty(unitName))
            {
                // Path assumes your unit prefabs are in Resources/Prefabs/Units/
                // Adjust "Prefabs/" to match your actual folder structure!
                UnitsInParty[i] = Resources.Load<GameObject>("Prefab/Units/Ally/" + unitName);
            }
            else
            {
                UnitsInParty[i] = null;
            }
        }
        Debug.Log("TeamData: Team composition loaded from disk.");
    }

    public void ResetTeam()
    {
        for (int i = 0; i < UnitsInParty.Count; i++)
        {
            UnitsInParty[i] = null;
            PlayerPrefs.DeleteKey(TEAM_KEY_PREFIX + i);
        }

        PlayerPrefs.Save();
        Debug.Log("TeamData: Team cleared and Save Data deleted.");
    }

    public void SortTeamAfterDeath()
    {
        // 1. Filter the list
        UnitsInParty.RemoveAll(unitGO =>
        {
            if (unitGO == null) return true;

            Unit unitScript = unitGO.GetComponent<Unit>();

            if (unitScript == null || unitScript.UnitData.CurrentHealthPoints <= 0)
            {
                return true;
            }

            return false;
        });

        while (UnitsInParty.Count < 4)
        {
            UnitsInParty.Add(null);
        }

        SaveTeam();
        Debug.Log("TeamData: Survivors shifted forward and saved.");
    }
}