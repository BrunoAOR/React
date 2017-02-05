using UnityEngine;

[System.Serializable]
public class LanguageTextElement
{
	[ShowOnly]
	[SerializeField]
	private Languages language;
	[Multiline(2)]
	public string text;

	public LanguageTextElement (Languages assignedLanguage) {
		language = assignedLanguage;
	}

	public void SetTextLanguage (Languages language) {
		this.language = language;
	}

	public Languages GetTextLanguage () {
		return (language);
	}
}
