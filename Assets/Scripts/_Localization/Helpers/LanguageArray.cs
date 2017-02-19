using UnityEngine;

[System.Serializable]
public class LanguageArray {

	public string keyword;
	[SerializeField]
	private LanguageTextElement[] languageTextElements = new LanguageTextElement[0];
	[SerializeField]
	private bool showLanguages;

	public LanguageArray (string[] languages) {
		SetUpLanguageArray (languages);
	}

	public void SetUpLanguageArray (string[] languages) {
		if (languageTextElements == null) {
			languageTextElements = new LanguageTextElement[languages.Length];
			for (int i = 0; i < languageTextElements.Length; i++) {
				languageTextElements [i] = new LanguageTextElement (languages [i]);
			}
		} else if (languages.Length != languageTextElements.Length) {
			ResizeArray (languages);
		} else {
			for (int i = 0; i < languageTextElements.Length; i++) {
				languageTextElements [i].SetTextLanguage (languages [i]);
			}
		}
	}

	public void ClearLanguageArray () {
		keyword = "";
		if (languageTextElements != null) {
			for (int i = 0; i < languageTextElements.Length; i++) {
				languageTextElements [i].text = "";
			}
		}
	}

	public int GetLanguagesCount () {
		return (languageTextElements.Length);
	}

	/// <summary>
	/// Gets the translation for the specific index..
	/// </summary>
	/// <returns>The translation for the selected index.</returns>
	/// <param name="languageIndex">Language index in the langauges list.</param>
	public string GetTranslation (int languageIndex) {
		if (languageIndex < 0) {
			languageIndex = 0;
		} else if (languageIndex >= languageTextElements.Length) {
			languageIndex = languageTextElements.Length - 1;
		}

		return (languageTextElements[languageIndex].text);
	}

	private void ResizeArray (string[] newLanguages) {
		if (languageTextElements.Length == newLanguages.Length) {
			return;
		}

		LanguageTextElement[] newLTE = new LanguageTextElement[newLanguages.Length];
		for (int i = 0; i < newLTE.Length; i++) {
			if (i < languageTextElements.Length) {
				newLTE [i] = languageTextElements [i];
				newLTE [i].SetTextLanguage (newLanguages [i]);
			} else {
				newLTE [i] = new LanguageTextElement (newLanguages [i]);
			}
		}
		languageTextElements = newLTE;
	}

}
