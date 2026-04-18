using UnityEngine;

[CreateAssetMenu(fileName = "CupEvent" , menuName = "ScriptableObjects / CupEvent")]
public class CupGame : RandomEvent
{
    public override void EventInit()
    {
        IsRandom = true;

        Option1Text = "Choose Cup";
        Option2Text = "Choose Cup";
        Option3Text = "Choose Cup";

        ScenarioDialogue = "Pick a cup";

        _currentImage = ScenarioImage;
        _currentDialogue = ScenarioDialogue;
    }
    public override void Option1Result(TeamData teamData)
    {
        int randomNumber = Random.Range(0, teamData.UnitsInParty.Count);
        Unit randomUnit = teamData.UnitsInParty[randomNumber].GetComponent<Unit>();
        randomUnit.UpdateHealth(5);
        ResultDialogue = "A random unit in your party healed 5HP";
    }

    public override void Option2Result(TeamData teamData)
    {
        int randomNumber = Random.Range(0, teamData.UnitsInParty.Count);
        Unit randomUnit = teamData.UnitsInParty[randomNumber].GetComponent<Unit>();
        randomUnit.UpdateHealth(-5);
        ResultDialogue = "A random unit in your party lost 5HP";
    }

    public override void Option3Result(TeamData teamData)
    {
        ResultDialogue = "Nothing happened";
    }

    public override void EventResult()
    {
        _currentImage = ResultImage;
        _currentDialogue = ResultDialogue;
    }
}
