using System;
using System.Collections;

public abstract class ActionState : TurnState
{
    public override void Enter()
    {
        base.Enter();
        StartCoroutine(HandleActions());
    }
    protected override void AddListeners()
    {
        base.AddListeners();
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
    }

    protected virtual void CounterConfirmed(object sender, InfoEventArgs<bool> e)
    {
        RemoveListeners();
    }

    protected abstract IEnumerator HandleActions();
}
