using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchPanDetector : GestureDetector
{
	public int NumberOfTouchesRequired = 1;
	public Vector2 Direction { get; private set; }

	private IDictionary<int, Touch> _touches = new Dictionary<int, Touch> ();

	private List<TouchPhase> downPhases = 
	new List<TouchPhase> { TouchPhase.Began, TouchPhase.Stationary, TouchPhase.Moved };

	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	public void Update ()
	{
		foreach (Touch touch in Input.touches) {
			_touches [touch.fingerId] = touch;
		}
		
		switch (State) {
		case GestureState.Inactive:
			checkForBeginning ();
			break;
		case GestureState.Began:
			updatePosition ();
			break;
		case GestureState.Complete:
			if (getNumberOfTouchesDown () == 0) {
				reset ();
			}
			break;
		}
	}

	protected void checkForBeginning ()
	{
		var touchesDownCount = getNumberOfTouchesDown ();
		
		//Debug.Log ("touchesDownCount: " + touchesDownCount);
		
		if (touchesDownCount >= NumberOfTouchesRequired) {
			State = GestureState.Began;
			StartPoint = CurrentPoint = Input.GetTouch (0).position;
		}
	}

	protected void updatePosition ()
	{
		var touchesDownCount = getNumberOfTouchesDown ();

		if (touchesDownCount < NumberOfTouchesRequired) {
			State = GestureState.Complete;
			CurrentPoint = EndPoint = Input.GetTouch (0).position;
		} else {
			CurrentPoint = Input.GetTouch (0).position;
			Direction = (CurrentPoint - StartPoint).normalized;
			Continue ();
		}
	}

	protected int getNumberOfTouchesDown ()
	{
		var touchesDownCount = 0;
		
		foreach (var kvp in _touches) {
			if (downPhases.Contains (kvp.Value.phase)) {
				touchesDownCount++;
			}
		}
		
		return touchesDownCount;
	}

	private void reset ()
	{
		StartPoint = CurrentPoint = EndPoint = Vector2.zero;
		State = GestureState.Inactive;
	}
}
