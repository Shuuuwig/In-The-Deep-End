using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfo : MonoBehaviour
{
    RoomData RoomData;

    public void RoomSetup(RoomData data)
    {
        RoomData = data;
        name = RoomData.RoomName;

        RoomHandler roomHandler = FindAnyObjectByType<RoomHandler>();
        MapHandler mapHandler = FindAnyObjectByType<MapHandler>();

        if (roomHandler != null)
        {
            GetComponent<Button>().onClick.AddListener(() => roomHandler.GoToRoom(RoomData));
            GetComponentInChildren<TMP_Text>().text = name;
        }
    }
}
