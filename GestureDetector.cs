using UnityEngine;
using System.Collections;

public class GestureDetector : MonoBehaviour
{

	public enum GestureState
	{
		Inactive,
		Began,
		Complete,
		Failed
	}
	public delegate void GestureHandler (GestureDetector detector,GameObject gameObject);

	public event GestureHandler OnBegin;
	public event GestureHandler OnContinue;
	public event GestureHandler OnComplete;
	public event GestureHandler OnFailed;

	public Vector2 StartPoint { get; protected set; }
	public Vector2 CurrentPoint { get; protected set; }
	public Vector2 EndPoint { get; protected set; }

	private GestureState _state;

	public GestureState State { 
		get { return _state; } 
		set { 
			_state = value; 

			if (_state == GestureState.Began) {
				if (OnBegin != null) {
					OnBegin (this, gameObject);
				}
			} else if (_state == GestureState.Complete) {
				if (OnComplete != null) {
					OnComplete (this, gameObject);
				}
			} else if (_state == GestureState.Failed) {
				if (OnFailed != null) {
					OnFailed (this, gameObject);
				}
			}
		}
	}

	protected void Continue ()
	{
		if (OnContinue != null)
			OnContinue (this, gameObject);
	}
}
