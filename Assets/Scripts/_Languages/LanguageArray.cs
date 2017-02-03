using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LanguageArray {

	public string keyword;
	[SerializeField]
	private LanguageTextElement[] languageTextElements;
	[SerializeField]
	private bool showLanguages;

	public LanguageArray () {
		SetUpLanguageArray ();
	}

	public void SetUpLanguageArray () {
		int languageCount = System.Enum.GetValues (typeof(Languages)).Length;
		if (languageTextElements == null || languageTextElements.Length != languageCount) {
			languageTextElements = new LanguageTextElement[languageCount];
			for (int i = 0; i < languageTextElements.Length; i++) {
				languageTextElements [i] = new LanguageTextElement ((Languages)i);
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

	/// <summary>
	/// Gets the translation for the currently selected language in the LanguageManager.
	/// </summary>
	/// <returns>The translation.</returns>
	public string GetTranslation () {
		return (languageTextElements [Managers.Language.GetLanguageIndex ()].text);
	}


	/// <summary>
	/// Gets the translation for the specified Language.
	/// </summary>
	/// <returns>The translation.</returns>
	/// <param name="language">Language (enum).</param>
	public string GetTranslation (Languages language) {
		return (GetTranslation ((int)language));
	}

	private string GetTranslation (int languageIndex) {
		if (languageIndex < 0) {
			languageIndex = 0;
		} else if (languageIndex >= Managers.Language.GetLanguagesCount ()) {
			languageIndex = Managers.Language.GetLanguagesCount () - 1;
		}

		return (languageTextElements[languageIndex].text);
	}

}
