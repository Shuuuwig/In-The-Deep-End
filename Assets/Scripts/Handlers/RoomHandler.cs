using UnityEngine;
using UnityEngine.SceneManagement;

public static class RoomHandler
{
    public static void GoToRoom (RoomData roomData)
    {
        SceneManager.LoadScene(roomData.RoomName);
        Debug.Log("ROOM");
    }
}
