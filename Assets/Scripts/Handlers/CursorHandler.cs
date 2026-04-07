using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        LockCursor();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}