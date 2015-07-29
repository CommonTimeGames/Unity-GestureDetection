/** 
The MIT License (MIT)

Copyright (c) 2015 Common Time Games

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

using UnityEngine;
using System.Collections;

public class MouseTapDetector : GestureDetector
{

	public double TimeLimit = 1.0;
	public double MovementTolerance = 50;
	public int MouseButton = 0;
	public Collider TargetCollider;
	public Camera Camera;
	
	private double _timer = 0.0;

	// Use this for initialization
	void Start ()
	{
		StartPoint = EndPoint = Vector2.zero;

		if (TargetCollider == null)
			TargetCollider = GetComponent<Collider> ();

		if (Camera == null)
			Camera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

		if (TargetCollider != null && Camera == null)
			Debug.LogWarning ("A collider is assigned, but not a camera. Collider tap detection will not work properly.");
	}
	
	// Update is called once per frame
	public void Update ()
	{
		switch (State) {
		case GestureState.Failed:
			State = GestureState.Inactive;
			break;
		case GestureState.Inactive:
			checkForBeginning ();
			break;
		case GestureState.Began:
			_timer -= Time.deltaTime;
			if (_timer <= 0) {
				State = GestureState.Failed;
				StartPoint = EndPoint = Vector2.zero;
			} else {
				checkForCompletion ();
			}
			break;
		case GestureState.Complete:
			State = GestureState.Inactive;
			break;
		}
	}

	protected virtual void checkForBeginning ()
	{
		if (Input.GetMouseButtonDown (MouseButton) && checkColliderHit ()) {
			StartPoint = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			State = GestureState.Began;
			_timer = TimeLimit;
		}
	}

	protected virtual void checkForCompletion ()
	{
		if (Input.GetMouseButtonUp (MouseButton)) {
			EndPoint = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			var difference = (EndPoint - StartPoint).magnitude;

			if (difference <= MovementTolerance && checkColliderHit ()) {
				State = GestureState.Complete;
			} else {
				State = GestureState.Failed;
			}
		}
	}

	protected virtual bool checkColliderHit ()
	{
		if (TargetCollider == null || Camera == null)
			return true;

		RaycastHit hit;

		if (!Physics.Raycast (Camera.ScreenPointToRay (Input.mousePosition), out hit, 100))
			return false;

		if (hit.collider == TargetCollider)
			return true;

		return false;
	}
}
