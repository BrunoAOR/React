using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Warning CS0649: Field '#fieldname#' is never assigned to, and will always have its default value null (CS0649) (Assembly-CSharp)
// Warning was raised for the following fields: _resetOnLanguageChange and _languageData
// Warning was disabled because these private fields are serialized and assigned through the inspector
#pragma warning disable 0649

public enum Languages {
	English,
	Spanish
}

public class LanguageManager : MonoBehaviour {

	[SerializeField]
	private Languages _activeLanguage;
	[SerializeField]
	private GameObject[] _resetOnLanguageChange;
	[SerializeField]
	private LanguageData _languageData;

	public void SetActiveLanguage (Languages newLanguage) {
		_activeLanguage = newLanguage;
		StartCoroutine (ResetUICanvas ());
	}

	public Languages GetActiveLanguage () {
		return _activeLanguage;
	}

	public int GetActiveLanguageIndex () {
		return (int)_activeLanguage;
	}

	public int GetLanguagesCount () {
		return (System.Enum.GetValues (typeof(Languages)).Length);

	}

	public string GetTranslation (string keyword) {
		return (_languageData.GetTranslation (keyword));
	}

	public int GetRequiredParametersCount (string keyword) {
		return (_languageData.GetRequiredParametersCount (keyword));
	}

	private IEnumerator ResetUICanvas () {
		for (int i = 0; i < _resetOnLanguageChange.Length; i++) {
			_resetOnLanguageChange[i].SetActive (false);
		}
		yield return null;
		for (int i = 0; i < _resetOnLanguageChange.Length; i++) {
			_resetOnLanguageChange[i].SetActive (true);
		}
	}
}
