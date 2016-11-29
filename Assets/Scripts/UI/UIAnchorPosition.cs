using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAnchorPosition : MonoBehaviour {
	private RectTransform _rectTransform;

	void Awake () {
		_rectTransform = GetComponent<RectTransform> ();
	}

	public void MoveToAnchorPoint (Vector2 anchorPoint) {
		_rectTransform.anchorMin = anchorPoint;
		_rectTransform.anchorMax = anchorPoint;
	}

	public void MoveToAnchorPointX (float x) {
		MoveToAnchorPoint (new Vector2 (x, _rectTransform.anchorMin.y));
	}

	public void MoveToAnchorPointY (float y) {
		MoveToAnchorPoint (new Vector2 (_rectTransform.anchorMin.x, y));
	}
}
