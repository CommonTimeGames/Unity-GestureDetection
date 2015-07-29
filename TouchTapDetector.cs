using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TouchTapDetector : GestureDetector
{

	public double TimeLimit = 1.0;
	public double MovementTolerance = 10;

	public int NumberOfTouchesRequired = 1;
	public int NumberOfTapsRequired = 1;

	public Collider TargetCollider;
	public Camera Camera;
	
	private IDictionary<int, Touch> _touches = new Dictionary<int, Touch> ();
	private IDictionary<int, Touch> _beginningTouches = new Dictionary<int, Touch> ();

	private bool _touchActive = false;
	private int numberOfTaps = 0;
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
			_timer -= Time.deltaTime;
			if (_timer <= 0) {
				State = GestureState.Failed;
				StartPoint = EndPoint = Vector2.zero;
				numberOfTaps = 0;
			} else {
				checkForCompletion ();
			}
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
			_timer = TimeLimit;
			_touchActive = true;
			_beginningTouches = new Dictionary<int, Touch> (_touches);
		}
	}

	protected void checkForCompletion ()
	{
		var touchesDownCount = getNumberOfTouchesDown ();

		var travelDistance = getTravelDistance ();

		if (travelDistance > MovementTolerance) {
			StartPoint = EndPoint = Vector2.zero;
			numberOfTaps = 0;
			State = GestureState.Failed;
		} else if (touchesDownCount == 0 && _touchActive) {
			_touchActive = false;
			numberOfTaps++;
			Debug.Log ("numberOfTaps: " + numberOfTaps);
		} else if (touchesDownCount >= NumberOfTouchesRequired) {
			_touchActive = true;
		}

		if (numberOfTaps >= NumberOfTapsRequired) {
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
		numberOfTaps = 0;
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
