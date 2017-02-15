using UnityEngine;

[System.Serializable]
public class LanguageTextElement
{
	[ShowOnly]
	[SerializeField]
	private string language;
	[Multiline(2)]
	public string text;

	public LanguageTextElement (string languageName) {
		language = languageName;
	}

	public void SetTextLanguage (string languageName) {
		language = languageName;
	}

	public string GetTextLanguage () {
		return (language);
	}
}
