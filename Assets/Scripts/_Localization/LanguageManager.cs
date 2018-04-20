using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Warning CS0649: Field '#fieldname#' is never assigned to, and will always have its default value null (CS0649) (Assembly-CSharp)
// Warning was raised for the following fields: _resetOnLanguageChange and _languageData
// Warning was disabled because these private fields are serialized and assigned through the inspector
#pragma warning disable 0649
// Warning CS0414: The private field '#fieldname#' is assigned but its value is never used (CS0414)
// Warning was raised for the following fields: _activeLanguage;
// Warning was disabled because this field is used only to show the language name (in the inspector) related to the language index that is used
#pragma warning disable 0414

public class LanguageManager : MonoBehaviour {

	[SerializeField]
	private int _activeLanguageIndex;
    [ShowOnly]
    [SerializeField]
	private string _activeLanguage;
	[SerializeField]
	private GameObject[] _resetOnLanguageChange;
	[SerializeField]
	private LanguageData _languageData;

	private void OnValidate () {
		_activeLanguageIndex = Mathf.Clamp (_activeLanguageIndex, 0, _languageData.GetLanguageCount () - 1);
		_activeLanguage = _languageData.GetLanguageName (_activeLanguageIndex);
	}

	public void SetActiveLanguage (int languageIndex) {
		languageIndex = Mathf.Clamp (languageIndex, 0, _languageData.GetLanguageCount () - 1);
		_activeLanguageIndex = languageIndex;
		_activeLanguage = _languageData.GetLanguageName (_activeLanguageIndex);
		StartCoroutine (ResetUICanvas ());
	}

	public int GetActiveLanguageIndex () {
		return (_activeLanguageIndex);
	}

	public int GetLanguagesCount () {
		return (_languageData.GetLanguageCount ());
	}

	public string GetTranslation (string keyword) {
		return (_languageData.GetTranslation (keyword));
	}

	public int GetRequiredParametersCount (string keyword) {
		return (_languageData.GetRequiredParametersCount (keyword));
	}

	private IEnumerator ResetUICanvas () {
		if (_resetOnLanguageChange == null || _resetOnLanguageChange.Length == 0) {
			yield break;
		}

		for (int i = 0; i < _resetOnLanguageChange.Length; i++) {
			_resetOnLanguageChange[i].SetActive (false);
		}

		yield return null;

		for (int i = 0; i < _resetOnLanguageChange.Length; i++) {
			_resetOnLanguageChange[i].SetActive (true);
		}
	}
}
