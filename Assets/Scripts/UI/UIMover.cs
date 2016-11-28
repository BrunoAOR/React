using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (RectTransform))]
public class UIMover : MonoBehaviour {
	private RectTransform _rectTransform;
	public Vector2 startAnchorMin;
	public Vector2 startAnchorMax;
	public float moveTime;
	public Vector2 targetAnchorMin;
	public Vector2 targetAnchorMax;


	void Awake () {
		_rectTransform = GetComponent<RectTransform> ();
	}

	public void SnapToStart () {
		_rectTransform.anchorMin = startAnchorMin;
		_rectTransform.anchorMax = startAnchorMax;
	}

	public IEnumerator MoveToTarget () {
		float startTime = Time.time;
		float u;
		float u2;
		float a = 0.15f;
		float b = (6 * a * a) - (6 * a) + 1;

		while ((Time.time - startTime) < moveTime) {
			u = (Time.time - startTime) / moveTime;

			u2 = - (2/b) * (u*u*u) + (3/b) * (u*u) + ((6*a*a/b) - (6*a/b)) * u;

			_rectTransform.anchorMin = Vector2.LerpUnclamped (startAnchorMin, targetAnchorMin, u2);
			_rectTransform.anchorMax = Vector2.LerpUnclamped (startAnchorMax, targetAnchorMax, u2);
			yield return null;
		}
		_rectTransform.anchorMin = targetAnchorMin;
		_rectTransform.anchorMax = targetAnchorMax;
		yield return null;
	}

}
