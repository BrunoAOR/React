using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LanguageArray {

	[SerializeField]
	private LanguageTextElement[] languageTextElements;

	public LanguageArray () {
	}

	public void SetUpLanguagesArray () {
		LanguageManager languageManager = GameObject.FindObjectOfType<LanguageManager> ();
		if (languageManager == null) {
			Debug.LogError ("No languageManager was found in the scene ");
			return;
		}
		if (languageTextElements == null) {
			languageTextElements = new LanguageTextElement[languageManager.GetLanguagesCount ()];
			for (int i = 0; i < languageTextElements.Length; i++) {
				languageTextElements [i] = new LanguageTextElement ((Languages)i);
			}
		} else if (languageTextElements.Length != languageManager.GetLanguagesCount ()) {
			LanguageTextElement[] previousArray = languageTextElements;
			languageTextElements = new LanguageTextElement[languageManager.GetLanguagesCount ()];
			for (int i = 0; i < languageTextElements.Length; i++) {
				languageTextElements [i] = new LanguageTextElement ((Languages)i);
				if (i < previousArray.Length) {
					languageTextElements [i].text = previousArray [i].text;
				}
			}
		}
	}

	public string GetTranslation () {
		return (languageTextElements [Managers.Language.GetLanguageIndex ()].text);
	}

	public string GetTranslation (Languages language) {
		return (GetTranslation ((int)language));
	}

	public string GetTranslation (int languageIndex) {
		return (languageTextElements[languageIndex].text);
	}
}
