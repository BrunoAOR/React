using UnityEngine;

[System.Serializable]
public class LanguageTextElement
{
	[ShowOnly]
	[SerializeField]
	private Languages language;
	[TextArea(2,3)]
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
