using UnityEngine;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

[CreateAssetMenu(fileName = "RoomData", menuName = "ScriptableObjects / RoomData")]
public class RoomData : ScriptableObject
{
    public RoomType RoomType;
    public string RoomName;
    public Sprite RoomIcon;
}
