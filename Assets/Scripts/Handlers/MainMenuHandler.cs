using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] Button _continueButton;
    [SerializeField] Button _newGameButton;
    [SerializeField] EventSystem _eventSystem;
    [SerializeField] MapData _mapData;
    [SerializeField] TeamData _teamData;
    [SerializeField] AudioClip _mainMenuMusic;
    [SerializeField] RoomData _prepRoomData;

    void Start()
    {
        if (_eventSystem == null)
        {
            _eventSystem = EventSystem.current;
        }

        if (_mapData == null)
        {
            _mapData = Resources.Load<MapData>("Maps/SavedMapData");
        }

        LoadMapDataFromPrefs();

        if (_teamData == null)
        {
            _teamData = Resources.Load<TeamData>("TeamData/PlayerTeamData");
        }

        LoadPlayerTeamDataFromPrefs();

        bool hasTeam = false;
        foreach (GameObject unit in _teamData.UnitsInParty)
        {
            if (unit != null)
            {
                hasTeam = true;
                break;
            }
        }

        if (_mapData.MapSeed != 0 && hasTeam)
        {
            _continueButton.interactable = true;
            _eventSystem.SetSelectedGameObject(_continueButton.gameObject);
        }
        else
        {
            _continueButton.interactable = false;
            _eventSystem.SetSelectedGameObject(_newGameButton.gameObject);
        }

        AudioHandler.Instance.PlayMusic(_mainMenuMusic, true);
    }

    void LoadMapDataFromPrefs()
    {
        if (PlayerPrefs.HasKey(MapData.KEY_SEED))
        {
            _mapData.MapSeed = PlayerPrefs.GetInt(MapData.KEY_SEED);
            _mapData.NumberOfRows = PlayerPrefs.GetInt(MapData.KEY_ROOMS);
            _mapData.CurrentRoom = PlayerPrefs.GetInt(MapData.KEY_CURRENT_ROOM);
            _mapData.CurrentRow = PlayerPrefs.GetInt(MapData.KEY_CURRENT_ROW);

            Debug.Log("Save data loaded successfully");
        }
    }

    void LoadPlayerTeamDataFromPrefs()
    {
        _teamData.LoadTeam();
    }

    public void ResetProgress()
    {
        _mapData.ResetProgress();

        // 1. Reset the stats of the current units BEFORE clearing the list
        foreach (GameObject prefab in _teamData.UnitsInParty)
        {
            if (prefab != null)
            {
                Unit unitScript = prefab.GetComponent<Unit>();
                if (unitScript != null && unitScript.UnitData != null)
                {
                    unitScript.UnitData.ResetCurrentStats();
                    Debug.Log($"Stats reset for: {unitScript.UnitData.name}");
                }
            }
        }

        _teamData.ResetTeam();
        
        SceneHandler.GoToRoom(_prepRoomData);
    }
}
