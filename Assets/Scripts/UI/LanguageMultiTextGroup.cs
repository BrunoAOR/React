using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class LanguageMultiTextGroup : MonoBehaviour {

	public Text connectedText;
	public TextGroup[] groups;

	[System.Serializable]
	public class TextGroup {
		public string groupName;
		public TextSubGroup[] subGroups;

		[System.Serializable]
		public class TextSubGroup {
			public string subGroupName;
			public LanguageArray languageArray;
		}
	}
		
	void Reset () {
		connectedText = GetComponent<Text> ();
	}

	void OnValidate () {
		if (groups != null && groups.Length > 0) {
			for (int g = 0; g < groups.Length; g++) {
				if (groups [g].subGroups != null && groups [g].subGroups.Length > 0) {
					for (int sg = 0; sg < groups [g].subGroups.Length; sg++) {
						groups [g].subGroups [sg].languageArray.SetUpLanguagesArray ();
					}
				}
			}
		}
	}


	public void SetUIText (string message) {
		if (connectedText == null)
			connectedText = GetComponent<Text> ();

		connectedText.text = message;
	}


	public void ClearText () {
		connectedText.text = "";
	}


	public string SelectUIText (int mainGroupIndex, int subGroupIndex) {
		if (connectedText == null)
			connectedText = GetComponent<Text> ();

		mainGroupIndex = Mathf.Clamp (mainGroupIndex, 0, groups.Length - 1);
		subGroupIndex = Mathf.Clamp (subGroupIndex, 0, groups [mainGroupIndex].subGroups.Length - 1);

		string selectedText = groups [mainGroupIndex].subGroups [subGroupIndex].languageArray.GetTranslation ();
		connectedText.text = selectedText;
		return selectedText;
	}
}
