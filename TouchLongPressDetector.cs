using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TouchLongPressDetector : GestureDetector
{
	
	public double PressTime = 1.0;
	public double MovementTolerance = 10;
	
	public int NumberOfTouchesRequired = 1;

	private IDictionary<int, Touch> _touches = new Dictionary<int, Touch> ();
	private IDictionary<int, Touch> _beginningTouches = new Dictionary<int, Touch> ();
	
	private double _timer = 0.0;	
	
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
			checkForCompletion ();
			break;
		case GestureState.Failed:
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
			_timer = PressTime;
			_beginningTouches = new Dictionary<int, Touch> (_touches);
			StartPoint = EndPoint = Input.GetTouch (0).position;
		}
	}
	
	protected void checkForCompletion ()
	{
		var touchesDownCount = getNumberOfTouchesDown ();

		if (touchesDownCount < NumberOfTouchesRequired) {
			StartPoint = EndPoint = Vector2.zero;
			State = GestureState.Failed;
			return;
		}

		var travelDistance = getTravelDistance ();

		if (travelDistance > MovementTolerance) {
			StartPoint = EndPoint = Vector2.zero;
			State = GestureState.Failed;
			return;
		} 

		_timer -= Time.deltaTime;
		
		if (_timer <= 0) {
			State = GestureState.Complete;
		}
	}
	
	private int getNumberOfTouchesDown ()
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
		StartPoint = EndPoint = Vector2.zero;
		State = GestureState.Inactive;
	}
	
	private float getTravelDistance ()
	{
		var maxDistance = 0.0F;
		
		foreach (var kvp in _touches) {
			var startTouch = _beginningTouches [kvp.Key];
			var endTouch = _touches [kvp.Key];
			
			var distance = (endTouch.position - startTouch.position).magnitude;
			maxDistance = distance > maxDistance ? distance : maxDistance;
		}
		
		return maxDistance;
	}
}
