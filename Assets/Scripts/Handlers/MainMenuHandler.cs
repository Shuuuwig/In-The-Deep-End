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

    void Awake()
    {


        if (_eventSystem == null)
            _eventSystem = EventSystem.current;

        if (_mapData == null)
            _mapData = Resources.Load<MapData>("Maps/SavedMapData");

        LoadMapDataFromPrefs();

        if (_mapData.MapSeed != 0)
        {
            _continueButton.interactable = true;
            _eventSystem.SetSelectedGameObject(_continueButton.gameObject);
        }
        else
        {
            _eventSystem.SetSelectedGameObject(_newGameButton.gameObject);
        }
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

    public void ResetProgress()
    {
        _mapData.ResetProgress();
    }
}
