using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TouchSwipeDetector : GestureDetector
{
	public int NumberOfTouchesRequired = 1;
	public float RequiredVelocity = 500;

	public Vector2 Direction { get; private set; }
	public SwipeDirection SwipeDirection { get; private set; }

	private List<SwipePoint> _touchHistory = new List<SwipePoint> ();
	private bool _touchDown;
	private float _velocity;

	private IDictionary<int, Touch> _touches = new Dictionary<int, Touch> ();
	
	private static readonly List<TouchPhase> downPhases = 
			new List<TouchPhase> { TouchPhase.Began, 
								   TouchPhase.Stationary, 
								   TouchPhase.Moved };
	
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach (Touch touch in Input.touches) {
			_touches [touch.fingerId] = touch;
		}

		_touchDown = getNumberOfTouchesDown () >= NumberOfTouchesRequired;
	
		if (!_touchDown && _touchHistory.Count > 0) {
			_touchHistory.Clear ();
		} else if (_touchDown) {

			var newPoint = new SwipePoint () {
				Position = Input.GetTouch (0).position,
				DeltaTime = Time.deltaTime,
				Velocity = 0
			};

			if (_touchHistory.Count > 0) {
				var lastPoint = _touchHistory.Last ();
				newPoint.Velocity = 
					(newPoint.Position - lastPoint.Position).magnitude / newPoint.DeltaTime;
			}

			_touchHistory.Add (newPoint);
		}

		switch (State) {
		case GestureState.Inactive:
			checkForBeginning ();
			break;
		case GestureState.Began:
			checkForCompletion ();
			break;
		case GestureState.Complete:
			reset ();
			break;
			
		case GestureState.Failed:
			if (!_touchDown)
				reset ();
			break;
		}
	}

	public void checkForBeginning ()
	{
		if (!_touchDown)
			return;

		var averageVelocity = getAverageTouchVelocity ();

		if (averageVelocity >= RequiredVelocity) {
			State = GestureState.Began;
			StartPoint = _touchHistory [0].Position;
		}
	}

	public void checkForCompletion ()
	{
		var averageVelocity = getAverageTouchVelocity ();

		if (averageVelocity < RequiredVelocity) {
			State = GestureState.Failed;
			StartPoint = EndPoint = CurrentPoint = Vector2.zero;
		} else if (!_touchDown) {
			State = GestureState.Complete;
			EndPoint = CurrentPoint;
		} else {
			CurrentPoint = _touchHistory [_touchHistory.Count - 1].Position;
			Continue ();
		}
	}

	protected float getAverageTouchVelocity ()
	{
		var velocitySum = 0.0F;
		
		foreach (var v in _touchHistory) {
			velocitySum += v.Velocity;
		}

		var avg = velocitySum / _touchHistory.Count;

		//Debug.Log ("averageVelocity: " + avg);

		return avg;
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

	protected void reset ()
	{
		State = GestureState.Inactive;
		StartPoint = CurrentPoint = EndPoint = Vector2.zero;
	}

}

public enum SwipeDirection
{
	Up,
	Down,
	Left,
	Right
}

public struct SwipePoint
{
	public Vector2 Position;
	public float DeltaTime;
	public float Velocity;
}
