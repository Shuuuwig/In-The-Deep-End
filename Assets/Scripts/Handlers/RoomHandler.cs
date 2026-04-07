using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static GameObject _Instance;

    public static void GoToRoom (RoomData roomData)
    {
        SceneManager.LoadScene(roomData.RoomName);
        Debug.Log("ROOM");
    }

    public static void GoToRoom (MapData mapData)
    {
        SceneManager.LoadScene(2);
        Debug.Log("ROOM");
    }

    public void QuitGame()
    {
        Application.Quit();
    } 
}

