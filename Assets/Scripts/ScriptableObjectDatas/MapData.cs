using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects / MapData")]
public class MapData : ScriptableObject
{
    public int MapSeed;
    public int NumberOfRows;
    public int CurrentRoom;
    public int CurrentRow;

    public const string KEY_SEED = "SavedMapSeed";
    public const string KEY_ROOMS = "SavedNumberOfRooms";
    public const string KEY_CURRENT_ROOM = "SavedCurrentRoom";
    public const string KEY_CURRENT_ROW = "SavedCurrentRow";

    public void AdvanceRow()
    {
        CurrentRow++;

        SaveProgress();
        Debug.Log($"MapData: Advanced to row {CurrentRow}. Progress saved to disk.");
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(KEY_SEED, MapSeed);
        PlayerPrefs.SetInt(KEY_ROOMS, NumberOfRows);
        PlayerPrefs.SetInt(KEY_CURRENT_ROOM, CurrentRoom);
        PlayerPrefs.SetInt(KEY_CURRENT_ROW, CurrentRow);

        PlayerPrefs.Save();
        Debug.Log("Combat Save Complete: Keys synced with MapData constants.");
    }

    public void ResetProgress()
    {
        MapSeed = 0;
        NumberOfRows = 0;
        CurrentRoom = -1;
        CurrentRow = -1;

        PlayerPrefs.DeleteKey(KEY_SEED);
        PlayerPrefs.DeleteKey(KEY_ROOMS);
        PlayerPrefs.DeleteKey(KEY_CURRENT_ROOM);
        PlayerPrefs.DeleteKey(KEY_CURRENT_ROW);

        PlayerPrefs.Save();

        Debug.Log("Global Progress Reset: ScriptableObject cleared and PlayerPrefs deleted.");
    }
}
