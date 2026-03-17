using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomHandler : MonoBehaviour
{
    public static GameObject _Instance;

    public static void GoToRoom (RoomData roomData)
    {
        SceneManager.LoadScene(roomData.RoomName);
        Debug.Log("ROOM");
    }
}
