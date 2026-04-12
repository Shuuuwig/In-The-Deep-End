using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static void GoToRoom (RoomData roomData)
    {
        SceneManager.LoadScene(roomData.RoomName);
        Debug.Log("ROOM");
    }

    public static void GoToMap ()
    {
        SceneManager.LoadScene(2);
        Debug.Log("To Map");
    }

    public void QuitGame()
    {
        Application.Quit();
    } 
}

