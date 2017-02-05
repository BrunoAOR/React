using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class LanguageMultiText : MonoBehaviour {

	[SerializeField]
	private Text connectedText;
	[SerializeField]
	private LanguageInfo languageInfo;
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
	public LanguageArray[] languageSets;

	private string activeKeyword;
	private int requiredParametersCount = 0;
	private string[] lastUsedParameters;
	private bool parametersAsKeywords = false;
	private bool calledFromParamsAsKeywords = false;

	// Fields used for data validation
	private int previousLanguageSetCount;
	private Dictionary<int,int> sameKeywords = new Dictionary<int, int> ();
	private List<int> emptyKeywords = new List<int> ();

	void Reset () {
		connectedText = GetComponent<Text> ();
		languageInfo = GetComponent<LanguageInfo> ();
		OnValidate ();
	}


	void Awake () {
		connectedText = GetComponent<Text> ();
		if (languageInfo == null) {
			languageInfo = GetComponent<LanguageInfo> ();
		}
		activeKeyword = languageSets [0].keyword;

		requiredParametersCount = CountMaxRequiredParameters (languageSets[0]);
	}

	void OnEnable () {
		CheckForRepeatedKeywords ();
		if (requiredParametersCount == 0) {
			Debugger ("1");
			ApplyLanguageTranslation (activeKeyword);
		} else if (lastUsedParameters != null) {
			if (parametersAsKeywords) {
				Debugger ("2");
				ApplyLanguageTranslationWithParametersAsKeywords (activeKeyword, lastUsedParameters);
			} else {
				Debugger ("3");
				ApplyLanguageTranslation (activeKeyword, lastUsedParameters);
			}
		}
	}

	private void Debugger (string id) {
		if (gameObject.name == "InformationText") {
			Debug.Log (gameObject.name + ": " + id);
		}
	}

	public void SetText (string message) {
		connectedText.text = message;
	}

	public Text GetConnectedText () {
		return connectedText;
	}

	void OnValidate () {
		if (connectedText == null) {
			connectedText = GetComponent<Text> ();
		}
		if (languageSets == null || languageSets.Length == 0) {
			languageSets = new LanguageArray[1];
			languageSets [0] = new LanguageArray ();

		}

		if (languageSets.Length > 0) {
			for (int i = 0; i < languageSets.Length; i++) {
				languageSets [i].SetUpLanguageArray ();
			}
		}

		if (previousLanguageSetCount > 0 && languageSets.Length > previousLanguageSetCount) {
			for (int i = previousLanguageSetCount; i < languageSets.Length; i++) {
				languageSets [i].ClearLanguageArray ();
			}
		}

		previousLanguageSetCount = languageSets.Length;

		CheckForRepeatedKeywords ();
	}

	public string GetLanguageTranslation (string keyword, params string[] parameters ) {
		int keywordIndex = FindKeywordIndex (keyword);
		if (keywordIndex == -1) {
			Debug.LogError (gameObject.name + "(LanguageMultiText): Keyword " + keyword + " could not be found in LanguageMultiText component in " + gameObject.name + " Game Object.");
			return (string.Empty);
		}

		requiredParametersCount = CountMaxRequiredParameters (languageSets[keywordIndex]);
		if (requiredParametersCount != 0 && (parameters == null || parameters.Length < requiredParametersCount)) {
			Debug.LogError (gameObject.name + "(LanguageMultiText): The amount of parameters passed to the ApplyLanguageTranslation method is insufficient");
			return (string.Empty);
		}

		activeKeyword = keyword;
		lastUsedParameters = parameters;

		string textToFormat = languageSets[keywordIndex].GetTranslation ();
		return (string.Format (textToFormat, parameters));
	}

	public void ApplyLanguageTranslationWithParametersAsKeywords (string keyword, params string[] parameters) {
		if (languageInfo == null) {
			Debug.LogError (gameObject.name + "(LanguageMultiText): ApplyLanguageTranslationWithParametersAsKeywords can't be used, because no languageInfo was set in the Inspector and no LanguageInfo component was found in the Game Object.");
			return;
		}

		if (parameters == null) {
			ApplyLanguageTranslation (keyword, parameters);
			return;
		}
			
		lastUsedParameters = parameters;
		parametersAsKeywords = true;
		string[] translatedParameters = new string[parameters.Length];

		for (int i = 0; i < parameters.Length; i++) {
			string translatedString = languageInfo.GetTranslation (parameters [i]);
			if (translatedString != string.Empty) {
				translatedParameters [i] = translatedString;
			} else {
				translatedParameters [i] = parameters [i];
			}
		}

		calledFromParamsAsKeywords = true;
		ApplyLanguageTranslation (keyword, translatedParameters);
	}

	public void ApplyLanguageTranslation (string keyword, params string[] parameters) {
		
		int keywordIndex = FindKeywordIndex (keyword);
		if (keywordIndex == -1) {
			connectedText.text = "Error: Keyword not found!";
			Debug.LogError (gameObject.name + "(LanguageMultiText): Keyword " + keyword + " could not be found in LanguageMultiText component in " + gameObject.name + " Game Object.");
			return;
		}

		requiredParametersCount = CountMaxRequiredParameters (languageSets[keywordIndex]);
		if (requiredParametersCount != 0 && (parameters == null || parameters.Length < requiredParametersCount)) {
			Debug.LogError (gameObject.name + "(LanguageMultiText): The amount of parameters passed to the ApplyLanguageTranslation method is insufficient");
			return;
		}

		activeKeyword = keyword;
		if (calledFromParamsAsKeywords) {
			calledFromParamsAsKeywords = false;
		} else {
			parametersAsKeywords = false;
			lastUsedParameters = parameters;
		}

		string textToFormat = languageSets[keywordIndex].GetTranslation ();
		connectedText.text = string.Format (textToFormat, parameters);
	}


	private void CheckForRepeatedKeywords () {
		if (languageSets.Length <= 1) {
			return;
		}

		List<int> newEmptyKeywords = new List<int> ();
		Dictionary<int, int> newSameKeywords = new Dictionary<int, int> ();
		for (int i = 0; i < languageSets.Length; i++) {
			if (languageSets [i].keyword == "") {
				newEmptyKeywords.Add (i);
			} else {
				for (int j = 0; j < i; j++) {
					if (languageSets [i].keyword == languageSets [j].keyword) {
						newSameKeywords.Add (i, j);
					}
				}
			}
		}


		bool emptyKeywordsChanged = false;
		if (emptyKeywords.Count == newEmptyKeywords.Count) {
			for (int i = 0; i < emptyKeywords.Count; i++) {
				if (emptyKeywords [i] != newEmptyKeywords [i]) {
					emptyKeywordsChanged = true;
					break;
				}
			}
		} else {
			emptyKeywordsChanged = true;
		}

		emptyKeywords = newEmptyKeywords;
		if (emptyKeywordsChanged && emptyKeywords.Count > 0) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			sb.Append (gameObject.name + "(LanguageMultiText): Some elements have empty keywords. Consider assigning ");
			if (emptyKeywords.Count == 1) {
				sb.Append ("a keyword for element ");
			} else {
				sb.Append ("keywords for elements ");
			}

			for (int i = 0; i < emptyKeywords.Count; i++) {
				if (i > 0) {
					if (i < emptyKeywords.Count - 1) {
						sb.Append (" ,");
					} else {
						sb.Append (" & ");
					}
				}
				sb.Append (emptyKeywords [i].ToString ());
			}
			sb.Append (".");

			Debug.LogWarning (sb.ToString ());
		}


		bool sameKeywordsChanged = false;
		if (sameKeywords.Count == newSameKeywords.Count) {
			foreach (int key in sameKeywords.Keys) {
				if (!newSameKeywords.ContainsKey (key) || sameKeywords[key] != newSameKeywords[key]) {
					sameKeywordsChanged = true;
					break;
				}
			}
		} else {
			sameKeywordsChanged = true;
		}

		sameKeywords = newSameKeywords;
		if (sameKeywordsChanged && sameKeywords.Keys.Count > 0) {
			foreach (KeyValuePair <int, int> kvp in sameKeywords) {
				Debug.LogError (gameObject.name + "(LanguageMultiText): Elements " + kvp.Value + " and " + kvp.Key + " have the same keyword (" + languageSets [kvp.Key].keyword + ")" + "\n"
					+ "Change the keyword in one of these elements to prevent element " + kvp.Key + " from never being selected."
				);
			}

		}

	}

	private bool KeywordExists (string keyword) {
		for (int i = 0; i < languageSets.Length; i++) {
			if (languageSets [i].keyword == keyword) {
				return (true);
			}
		}
		return (false);
	}

	private int FindKeywordIndex (string keyword) {
		for (int i = 0; i < languageSets.Length; i++) {
			if (languageSets [i].keyword == keyword) {
				return (i);
			}
		}
		return -1;
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
