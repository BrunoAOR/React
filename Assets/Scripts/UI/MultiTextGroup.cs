using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiTextGroup : MonoBehaviour {

	[System.Serializable]
	public class TextGroup {
		public string groupName;
		[TextArea (1,3)]
		public string[] texts;
	}

	public TextGroup[] groups;

	private Text _text;

	void Awake () {
		_text = GetComponent<Text> ();
	}


	public void SetUIText (string message) {
		if (_text == null)
			_text = GetComponent<Text> ();
		
		_text.text = message;
	}

	public string SelectUIText (int mainGroupIndex, int subGroupIndex) {
		if (_text == null)
			_text = GetComponent<Text> ();

		mainGroupIndex = Mathf.Clamp (mainGroupIndex, 0, groups.Length - 1);
		subGroupIndex = Mathf.Clamp (subGroupIndex, 0, groups [mainGroupIndex].texts.Length - 1);

		string selectedText = groups [mainGroupIndex].texts [subGroupIndex];
		_text.text = selectedText;
		return selectedText;
	}
}
