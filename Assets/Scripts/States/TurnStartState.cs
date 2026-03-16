using System.Collections;
using UnityEngine;

public class TurnStartState : TurnState
{
    public override void Enter()
    {
        base.Enter();
    }

    protected override IEnumerator CheckStatus()
    {
        yield return null;
    }
}
