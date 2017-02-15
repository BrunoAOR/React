using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Localization/Language Data", fileName = "New Language Data")]
public class LanguageData : ScriptableObject {


	[SerializeField]
	private string[] languages;

	[SerializeField]
	private LanguageArray[] languageSets;

	// Fields used for data validation in edit-time
	private int previousLanguageSetCount;
	private Dictionary<int,int> previousSameKeywords = new Dictionary<int, int> ();
	private List<int> previousEmptyKeywords = new List<int> ();
	private bool okDisplayed = true;

	private void OnValidate () {
		if (languageSets == null || languageSets.Length == 0) {
			languageSets = new LanguageArray[1];
			languageSets [0] = new LanguageArray (languages);

		}

		if (languageSets.Length > 0) {
			for (int i = 0; i < languageSets.Length; i++) {
				languageSets [i].SetUpLanguageArray (languages);
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

	public int GetLanguageCount () {
		return (languages.Length);
	}

	public string GetLanguageName (int languageIndex) {
		if (languageIndex < 0 || languageIndex >= languages.Length) {
			return (null);
		} else {
			return (languages [languageIndex]);
		}
	}

	public string GetTranslation (string keyword) {
		int keywordIndex = GetKeywordIndex (keyword);
		if (keywordIndex == -1) {
			return (null);
		}
		return (languageSets[keywordIndex].GetTranslation (Managers.Language.GetActiveLanguageIndex ()));
	}

	public int GetRequiredParametersCount (string keyword) {
		int index = GetKeywordIndex (keyword);
		if (index == -1) {
			return (-1);
		} else {
			return CountMaxRequiredParameters (languageSets [index]);
		}
	}

	private bool KeywordExists (string keyword) {
		if (GetKeywordIndex (keyword) == -1) {
			return (false);
		} else {
			return (true);
		}
	}

	private int GetKeywordIndex (string keyword) {
		for (int i = 0; i < languageSets.Length; i++) {
			if (languageSets [i].keyword == keyword) {
				return (i);
			}
		}
		return (-1);
	}

	private void CheckForRepeatedKeywords () {
		// Return if there is only one element in the data
		if (languageSets.Length <= 1) {
			return;
		}

		// Create 2 collections to check for empty and repeateed keywords
		List<int> emptyKeywords = new List<int> ();
		Dictionary<int, int> sameKeywords = new Dictionary<int, int> ();

		// Find all empty or repeated keywords
		for (int i = 0; i < languageSets.Length; i++) {
			if (languageSets [i].keyword == "") {
				emptyKeywords.Add (i);
			} else {
				for (int j = 0; j < i; j++) {
					if (languageSets [i].keyword == languageSets [j].keyword) {
						sameKeywords.Add (i, j);
					}
				}
			}
		}

		// If no empty or repeatedKeywords were found...
		if (emptyKeywords.Count == 0 && sameKeywords.Count == 0) {
			if (!okDisplayed) {
				okDisplayed = true;
				Debug.Log ("LanguageData: NO Errors. All elements have unique, non-empty keywords.");
			}
			// Store the new (empty) collections and return
			previousEmptyKeywords = emptyKeywords;
			previousSameKeywords = sameKeywords;
			return;
		} else {
			okDisplayed = false;
		}

		// Create and log a message for empty keywords
		bool emptyKeywordsChanged = false;
		if (previousEmptyKeywords.Count == emptyKeywords.Count) {
			for (int i = 0; i < previousEmptyKeywords.Count; i++) {
				if (previousEmptyKeywords [i] != emptyKeywords [i]) {
					emptyKeywordsChanged = true;
					break;
				}
			}
		} else {
			emptyKeywordsChanged = true;
		}

		previousEmptyKeywords = emptyKeywords;
		if (emptyKeywords.Count > 0 && emptyKeywordsChanged) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			sb.Append ("LanguageData: Some elements have empty keywords. Assign ");
			if (emptyKeywords.Count == 1) {
				sb.Append ("a keyword for element ");
			} else {
				sb.Append ("keywords for elements ");
			}

			for (int i = 0; i < emptyKeywords.Count; i++) {
				if (i > 0) {
					if (i < emptyKeywords.Count - 1) {
						sb.Append (", ");
					} else {
						sb.Append (" & ");
					}
				}
				sb.Append (emptyKeywords [i].ToString ());
			}
			sb.Append (".");

			Debug.LogError (sb.ToString ());
		}

		// Create and log a message for empty keywords
		bool sameKeywordsChanged = false;
		if (previousSameKeywords.Count == sameKeywords.Count) {
			foreach (int key in previousSameKeywords.Keys) {
				if (!sameKeywords.ContainsKey (key) || previousSameKeywords[key] != sameKeywords[key]) {
					sameKeywordsChanged = true;
					break;
				}
			}
		} else {
			sameKeywordsChanged = true;
		}

		previousSameKeywords = sameKeywords;
		if (sameKeywords.Keys.Count > 0 && sameKeywordsChanged) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			sb.Append ("LanguageData: Some element pairs have the same keywords. Change the keyword for one element of the following ");
			if (sameKeywords.Count == 1) {
				sb.Append ("pair: ");
			} else {
				sb.Append ("pairs: ");
			}
			int count = 0;
			foreach (KeyValuePair <int, int> kvp in sameKeywords) {
				if (count > 0) {
					if (count < sameKeywords.Count - 1) {
						sb.Append (", ");
					} else {
						sb.Append (" and ");
					}
				}
				sb.Append (kvp.Value + " & " + kvp.Key);
				count++;
			}
			sb.Append (".");
			Debug.LogError (sb.ToString ());
		}
	}

	private int CountMaxRequiredParameters (LanguageArray languageArray) {
		int count = 0;
		string testText;
		for (int i = 0; i < languageArray.GetLanguagesCount (); i++) {
			testText = languageArray.GetTranslation (i);
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
