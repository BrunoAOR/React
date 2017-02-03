using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class LanguageText : MonoBehaviour {

	public Text connectedText;
	public LanguageArray languages;

	void Reset () {
		connectedText = GetComponent<Text> ();
		languages = new LanguageArray ();
	}

	void OnEnable () {
		ApplyLanguageTranslation ();
	}

	private void ApplyLanguageTranslation () {
		connectedText.text = languages.GetTranslation();
	}
}
