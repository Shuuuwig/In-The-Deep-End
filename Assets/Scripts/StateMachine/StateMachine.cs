using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected State _newState;
    protected State _currentState;
    public State CurrentState { get { return _currentState; } }

    public virtual void ChangeState<T>() where T : State
    {
        _newState = GetComponent<T>();

        if (_newState == null)
            _newState = gameObject.AddComponent<T>();

        TransitionState(_newState);
    }

    protected virtual void TransitionState(State newState)
    {
        if (newState == _currentState)
            return;

        if (_currentState != null)
            _currentState.Exit();

        _currentState = newState;

        if (_currentState != null)
            _currentState.Enter();
    }
}