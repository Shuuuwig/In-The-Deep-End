using UnityEngine;

public class State : MonoBehaviour
{
    
    public virtual void Enter ()
	{
		AddListeners();
	}
	
	public virtual void Exit ()
	{
		RemoveListeners();
	}
	protected virtual void AddListeners ()
	{
	}
	
	protected virtual void RemoveListeners ()
	{
	}

}
