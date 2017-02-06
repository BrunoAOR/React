﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Warning CS0649: Field '#fieldname#' is never assigned to, and will always have its default value null (CS0649) (Assembly-CSharp)
// Warning was raised for the following fields: resetOnLanguageChange
// Warning was disabled because these private fields are serialized and assigned through the inspector
#pragma warning disable 0649

public enum Languages {
	English,
	Spanish
}

public class LanguageManager : MonoBehaviour {

	[SerializeField]
	private Languages language;
	[SerializeField]
	private GameObject[] resetOnLanguageChange;

	public void SetLanguage (Languages newLanguage) {
		language = newLanguage;
		StartCoroutine (ResetUICanvas ());
	}

	public Languages GetLanguage () {
		return language;
	}

	public int GetLanguageIndex () {
		return (int)language;
	}

	public int GetLanguagesCount () {
		bool foundLast = false;
		int index = -1;

		while (!foundLast) {
			index++;
			if (((Languages)index).ToString () == index.ToString ()) {
				foundLast = true;
			}
			if (index == 50) {
				foundLast = true;
				Debug.LogError ("GetLanguageCount() is capped at 50. If you actually have over 50 elements, increase the cap.");
			}
		}

		return index;
	}

	private IEnumerator ResetUICanvas () {
		for (int i = 0; i < resetOnLanguageChange.Length; i++) {
			resetOnLanguageChange[i].SetActive (false);
		}
		yield return null;
		for (int i = 0; i < resetOnLanguageChange.Length; i++) {
			resetOnLanguageChange[i].SetActive (true);
		}
	}
}