using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class LanguageText : MonoBehaviour {


	[SerializeField]
	private Text connectedText;
	public Color color {
		get {
			return (connectedText.color);
		}
		set {
			connectedText.color = value;
		}
	}
	public RectTransform rectTransform {
		get {
			return (connectedText.rectTransform);
		}
	}
	public LanguageArray languages;

	private int requiredParametersCount = 0;

	void Reset () {
		connectedText = GetComponent<Text> ();
		languages = new LanguageArray ();
	}

	void Awake () {
		connectedText = GetComponent<Text> ();
		if (languages == null) {
			languages = new LanguageArray ();
		}
		requiredParametersCount = CountMaxRequiredParameters (languages);
	}

	void OnEnable () {
		if (requiredParametersCount == 0) {
			ApplyLanguageTranslation ();
		}
	}

	public void SetText (string message) {
		connectedText.text = message;
	}

	public Text GetConnectedText () {
		return connectedText;
	}

	public string GetLanguageTranslation (params string[] parameters) {
		if (requiredParametersCount != 0 && (parameters == null || parameters.Length < requiredParametersCount)) {
			Debug.LogError (gameObject.name + "(LanguageText): The amount of parameters passed to the ApplyLanguageTranslation method is insufficient" + requiredParametersCount);
			return (string.Empty);
		}
		string textToFormat = languages.GetTranslation ();
		return (string.Format(textToFormat, parameters));
	}

	private void ApplyLanguageTranslation (params string[] parameters) {
		if (requiredParametersCount != 0 && (parameters == null || parameters.Length < requiredParametersCount)) {
			Debug.LogError (gameObject.name + "(LanguageText): The amount of parameters passed to the ApplyLanguageTranslation method is insufficient" + requiredParametersCount);
			return;
		}

		string textToFormat = languages.GetTranslation ();
		connectedText.text = string.Format (textToFormat, parameters);
	}

	private int CountMaxRequiredParameters (LanguageArray languageArray) {
		int count = 0;
		string testText;
		for (int i = 0; i < languageArray.GetLanguagesCount (); i++) {
			testText = languageArray.GetTranslation ((Languages)i);
			count = Mathf.Max (count, CountParameters(testText));
		}
		return (count);
	}

	private int CountParameters (string testString) {
		int maxParamIndex = -1;
		string subString;
		int parsedInt;
		for (int c = 0; c < testString.Length; c++) {
			if (testString [c] == '{' && c < testString.Length - 2) {
				if (testString [c + 1] == '{') {
					// Skip the next character which is a '{' because the double '{' does not count as a parameters
					c++;
				} else {
					// Now we look for the closing bracket.
					for (int t = c + 1; t < testString.Length; t++) {
						if (testString [t] == '}') {
							// We found the closing bracket, so now we check if the enclosed string is an int
							// We get the substring enclosed in the brackets
							subString = testString.Substring (c + 1, t - (c + 1));
							// We try to parse it to an Int
							if (int.TryParse (subString, out parsedInt)) {
								// If successful, we compare the value (which represent the parameter index) with the current maxParamIndex and store the maximum value.
								maxParamIndex = Mathf.Max (maxParamIndex, parsedInt);
							}
							break;
						}
					}
				}
			}
		}

		// We add one because the parameter indexes start at 0
		return (maxParamIndex + 1);
	}
}
