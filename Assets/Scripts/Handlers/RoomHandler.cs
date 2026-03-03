using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomHandler : MonoBehaviour
{
    public void GoToRoom (RoomData roomData)
    {
        SceneManager.LoadScene(roomData.RoomName);
        Debug.Log("ROOM");
    }
}
