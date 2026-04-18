using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static void GoToRoom(RoomData roomData)
    {
        SceneManager.LoadScene(roomData.RoomName);
        Debug.Log("ROOM");
    }

    public static void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public static void GoToMap()
    {
        SceneManager.LoadScene(2);
        Debug.Log("To Map");
    }

    public static void GameOver()
    {
        SceneManager.LoadScene(3);
        Debug.Log("GameOver");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

