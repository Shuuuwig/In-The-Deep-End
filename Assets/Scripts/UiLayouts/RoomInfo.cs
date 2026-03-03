using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] Button _roomButton;
    RoomData _roomData;
    public List<RoomInfo> NextConnectedRooms = new List<RoomInfo>();

    public void RoomSetup(MapData mapData, RoomData roomData, MapHandler mapHandler, RoomHandler roomHandler)
    {
        _roomData = roomData;
        name = _roomData.RoomName;
        
        if (_roomButton == null)
            _roomButton = GetComponent<Button>();

        _roomButton.onClick.RemoveAllListeners();

        if (roomHandler != null)
        {
            _roomButton.onClick.AddListener(() => roomHandler.GoToRoom(_roomData));
            mapData.CurrentRow++;
        }
            

        if (mapHandler != null)
            _roomButton.onClick.AddListener(() => mapHandler.EnterRoom(this));

        TMP_Text displayName = GetComponentInChildren<TMP_Text>();
        if (displayName != null)
            displayName.text = name;
    }

    public void SetInteractable(bool state)
    {
        if (_roomButton != null)
            _roomButton.interactable = state;
    }
}