using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] Button _roomButton;
    RoomData _roomData;
    public List<RoomInfo> NextConnectedRooms = new List<RoomInfo>();

    // Property to check interactable state (helpful for MapHandler loops)
    public bool IsInteractable => _roomButton != null && _roomButton.interactable;

    public void RoomSetup(MapData mapData, RoomData roomData, MapHandler mapHandler)
    {
        _roomData = roomData;
        name = _roomData.RoomName;

        if (_roomButton == null)
            _roomButton = GetComponent<Button>();

        Navigation nav = _roomButton.navigation;
        nav.mode = Navigation.Mode.Automatic;
        _roomButton.navigation = nav;

        ColorBlock cb = _roomButton.colors;
        cb.selectedColor = new Color(1f, 0.5f, 0f);
        cb.highlightedColor = new Color(1f, 0.6f, 0.1f); 
        _roomButton.colors = cb;

        // ---------------------------------------

        _roomButton.onClick.RemoveAllListeners();
        _roomButton.onClick.AddListener(() =>
        {
            // Note: MapHandler already increments CurrentRow in EnterRoom()
            // but keeping your logic here if you prefer it in RoomInfo.
            if (mapHandler != null)
            {
                mapHandler.EnterRoom(this);
            }

            Debug.Log("To a room: " + _roomData.RoomName);
            SceneHandler.GoToRoom(_roomData);
        });

        TMP_Text displayName = GetComponentInChildren<TMP_Text>();
        if (displayName != null)
            displayName.text = name;
    }

    public void SetInteractable(bool state)
    {
        if (_roomButton == null)
            _roomButton = GetComponent<Button>();

        if (_roomButton != null)
        {
            _roomButton.interactable = state;
        }
        else
        {
            Debug.LogWarning("MISSING ROOM BUTTON ON " + gameObject.name);
        }
    }
}