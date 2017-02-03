using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class LanguageMultiText : MonoBehaviour {

	public Text connectedText;
	public LanguageArray[] languageSets;
	private string activeKeyword;

	// Fields used for data validation
	private int previousLanguageSetCount;
	private Dictionary<int,int> sameKeywords = new Dictionary<int, int> ();
	private List<int> emptyKeywords = new List<int> ();

	void Reset () {
		connectedText = GetComponent<Text> ();
		OnValidate ();
	}

	void OnEnable () {
		CheckForRepeatedKeywords ();
		ApplyLanguageTranslation ();
	}

	void OnValidate () {
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

	void OnAwake () {
		activeKeyword = languageSets [0].keyword;
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
				Debug.LogError ("Elements " + kvp.Value + " and " + kvp.Key + " have the same keyword (" + languageSets [kvp.Key].keyword + ")" + "\n"
					+ "Change the keyword in one of these elements to prevent element " + kvp.Key + " from never being selected."
				);
			}

		}

	}

	private void ApplyLanguageTranslation (int idx = 0) {
		connectedText.text = languageSets[idx].GetTranslation();
	}

	public void SetActive (bool _) {
		gameObject.SetActive (_);
		ApplyLanguageTranslation (activeKeyword);
	}

	public void ApplyLanguageTranslation (string keyword) {
		int index = FindKeywordIndex (keyword);
		if (index != -1) {
			activeKeyword = keyword;
			ApplyLanguageTranslation (index);
		} else {
			connectedText.text = "Error: Keyword not found!";
			Debug.LogWarning ("Keyword " + keyword + " could not be found in LanguageMultiText component in " + gameObject.name + " Game Object.");
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
}
