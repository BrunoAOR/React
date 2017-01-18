using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILocalPosition : MonoBehaviour {

	public float moveDuration;
	public Vector2 startPosition;
	public Vector2 targetPosition;

	private RectTransform _rectTransform;

	void Awake () {
		_rectTransform = GetComponent<RectTransform> ();
	}

	public void SnapToStart () {
		_rectTransform.localPosition = startPosition;
	}
	
	public IEnumerator MoveToTarget () {
		float startTime = Time.time;
		float u;
		float u2;
		float a = 0.15f;
		float b = (6 * a * a) - (6 * a) + 1;

		while ((Time.time - startTime) < moveDuration) {
			u = (Time.time - startTime) / moveDuration;
			u2 = - (2/b) * (u*u*u) + (3/b) * (u*u) + ((6*a*a/b) - (6*a/b)) * u;

			_rectTransform.localPosition = Vector3.LerpUnclamped (startPosition, targetPosition, u2);
			yield return null;
		}

		_rectTransform.localPosition = targetPosition;
	}
}
