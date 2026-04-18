using System;
using UnityEngine;

public abstract class RandomEvent : ScriptableObject
{
    public Sprite ScenarioImage;
    public Sprite ResultImage;
    public string ScenarioDialogue;
    public string ResultDialogue;
    public string Option1Text;
    public string Option2Text;
    public string Option3Text;
    public bool IsRandom;
    
    protected Sprite _currentImage;
    protected string _currentDialogue;

    public abstract void EventInit();
    public abstract void Option1Result(TeamData teamData);
    public abstract void Option2Result(TeamData teamData);
    public abstract void Option3Result(TeamData teamData);
    public abstract void EventResult();
}
