using UnityEngine;
using UnityEditor;

public class ClearPrefs
{
    [MenuItem("Tools/Clear All PlayerPrefs")]
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}