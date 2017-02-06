using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class LanguageText : MonoBehaviour {

	[SerializeField]
	private Text connectedText;
	public string activeKeyword;
	public bool parametersAsKeywords = false;
	public string[] activeParameters;

	public RectTransform rectTransform {
		get {
			return (connectedText.rectTransform);
		}
	}
	public Color color {
		get {
			return (connectedText.color);
		}
		set {
			connectedText.color = value;
		}
	}

	private int requiredParametersCount = 0;

	private void Reset () {
		if (connectedText == null) {
			connectedText = GetComponent<Text> ();
		}
	}

	private void Awake () {
		if (connectedText == null) {
			connectedText = GetComponent<Text> ();
		}
	}

	private void OnEnable () {
		if (string.IsNullOrEmpty (activeKeyword)) {
			connectedText.text = "ERROR: No keyword";
			return;
		}

		if (requiredParametersCount == 0) {
			ApplyTranslation (activeKeyword);
		} else if (activeParameters != null) {
			if (parametersAsKeywords) {
				ApplyTranslation (activeKeyword, true, activeParameters);
			} else {
				ApplyTranslation (activeKeyword, false, activeParameters);
			}
		}
	}

	public void SetText (string message) {
		connectedText.text = message;
	}

	public Text GetConnectedText () {
		return connectedText;
	}

	public string GetTranslation (bool tryParametersAsKeywords = false, params string[] parameters) {
		return GetTranslation (activeKeyword, tryParametersAsKeywords, parameters);
	}

	public string GetTranslation (string keyword, bool tryParametersAsKeywords = false, params string[] parameters) {

		string translation = Managers.Language.GetTranslation (keyword);

		if (translation == null) {
			Debug.LogError (gameObject.name + "(LanguageText): Keyword " + keyword + " does not exist.");
			return (null);
		}

		requiredParametersCount = Managers.Language.GetRequiredParametersCount(keyword);
		if (requiredParametersCount != 0 && (parameters == null || parameters.Length < requiredParametersCount)) {
			Debug.LogError (gameObject.name + "(LanguageText): The amount of parameters passed to the GetTranslation method is insufficient");
			return (null);
		}

		if (tryParametersAsKeywords) {
			parametersAsKeywords = true;
			string[] translatedParameters = new string[parameters.Length];

			for (int i = 0; i < parameters.Length; i++) {
				string translatedString = Managers.Language.GetTranslation (parameters [i]);
				if (translatedString != string.Empty) {
					translatedParameters [i] = translatedString;
				} else {
					translatedParameters [i] = parameters [i];
				}
			}
			connectedText.text = string.Format (translation, translatedParameters);
		} else {
			parametersAsKeywords = false;
			connectedText.text = string.Format (translation, parameters);
		}
		return (string.Format (translation, parameters));
	}

	public void ApplyTranslation (bool tryParametersAsKeywords = false, params string[] parameters) {
		ApplyTranslation (activeKeyword, tryParametersAsKeywords, parameters);
	}

	public void ApplyTranslation (string keyword, bool tryParametersAsKeywords = false, params string[] parameters) {

		string translation = Managers.Language.GetTranslation (keyword);

		if (translation == null) {
			Debug.LogError (gameObject.name + "(LanguageText): Keyword " + keyword + " does not exist.");
			return;
		}

		requiredParametersCount = Managers.Language.GetRequiredParametersCount(keyword);
		if (requiredParametersCount != 0 && (parameters == null || parameters.Length < requiredParametersCount)) {
			Debug.LogError (gameObject.name + "(LanguageText): The amount of parameters passed to the ApplyTranslation method is insufficient");
			return;
		}

		activeKeyword = keyword;
		activeParameters = parameters;
		if (tryParametersAsKeywords) {
			parametersAsKeywords = true;
			string[] translatedParameters = new string[parameters.Length];

			for (int i = 0; i < parameters.Length; i++) {
				string translatedString = Managers.Language.GetTranslation (parameters [i]);
				if (translatedString != null) {
					translatedParameters [i] = translatedString;
				} else {
					translatedParameters [i] = parameters [i];
				}
			}
			connectedText.text = string.Format (translation, translatedParameters);
		} else {
			parametersAsKeywords = false;
			connectedText.text = string.Format (translation, parameters);
		}

	}

}
