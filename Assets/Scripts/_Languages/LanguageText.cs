using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class LanguageText : MonoBehaviour {

	public Text connectedText;
	public LanguageArray languageArray;

	void Reset () {
		connectedText = GetComponent<Text> ();
		languageArray = new LanguageArray ();
		OnValidate ();

	}

	void OnValidate () {
		languageArray.SetUpLanguagesArray ();
	}

	void OnEnable () {
		ApplyLanguageTranslation ();
	}

	private void ApplyLanguageTranslation () {
		connectedText.text = languageArray.GetTranslation();
	}
}
