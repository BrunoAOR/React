using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuFullStatsButton : MonoBehaviour {

	private RectTransform _rectTransform;
	private Text _text;

	void Awake () {
		_rectTransform = GetComponent<RectTransform> ();
		_text = GetComponentInChildren<Text> ();
	}

	public void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	public void SetText (string message) {
		_text.text = message;
	}

	public void ShiftAnchoredPosition (Vector2 shift) {
		_rectTransform.anchoredPosition += shift;

	}

	public void ShiftAnchoredPosition (float xShift, float yShift) {
		ShiftAnchoredPosition (new Vector2 (xShift, yShift));
	}
}
