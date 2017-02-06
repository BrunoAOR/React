using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuFullStatsButton : MonoBehaviour {

	private RectTransform _rectTransform;
	private LanguageText _languageText;

	void Awake () {
		_rectTransform = GetComponent<RectTransform> ();
		_languageText = GetComponentInChildren<LanguageText> ();
	}

	public void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	public void ApplyTranslation (string keyword) {
		_languageText.ApplyTranslation(keyword);
	}

	public void ShiftAnchoredPosition (Vector2 shift) {
		_rectTransform.anchoredPosition += shift;
	}

	public void ShiftAnchoredPosition (float xShift, float yShift) {
		ShiftAnchoredPosition (new Vector2 (xShift, yShift));
	}
}
