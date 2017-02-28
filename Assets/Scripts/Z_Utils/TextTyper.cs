using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper {

	private WaitForSeconds _waitZero = new WaitForSeconds (0.001f);
	private WaitForSeconds _waitAfterChar;
	private WaitForSeconds _waitAfterSpace;
	private WaitForSeconds _waitAfterComma;
	private WaitForSeconds _waitAfterPoint;
	private float _minPitch = 0.8f;
	private float _maxPitch = 1.2f;
	private bool _rushTyping = false;
	private bool _abortTyping = false;

	private System.Text.StringBuilder _stringBuilder = new System.Text.StringBuilder ();

	public TextTyper (float minimumPitch = 0.8f, float maximumPitch = 1.2f, float waitAfterChar = 0.05f, float waitAfterSpace = 0.1f, float waitAfterComma = 0.1f, float waitAfterPoint = 0.5f) {
		SetAudioPitchParameters (minimumPitch, maximumPitch);
		SetWaitParameters (waitAfterChar, waitAfterSpace, waitAfterComma, waitAfterPoint);
	}

	public void SetWaitParameters (float waitAfterChar, float waitAfterSpace, float waitAfterComma, float waitAfterPoint) {
		_waitAfterChar = new WaitForSeconds (waitAfterChar);
		_waitAfterSpace = new WaitForSeconds (waitAfterSpace);
		_waitAfterComma = new WaitForSeconds (waitAfterComma);
		_waitAfterPoint = new WaitForSeconds (waitAfterPoint);
	}

	public void SetAudioPitchParameters (float minimumPitch, float maximumPitch) {
		_minPitch = minimumPitch;
		_maxPitch = maximumPitch;
	}

	/// <summary>
	/// Types textToType string character by character in the textField. When typing each character, the typingClip is played as OneShot using the audioSource
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="caller">Caller.</param>
	/// <param name="textField">Text field.</param>
	/// <param name="textToType">Text to type.</param>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="typingClip">Typing clip.</param>
	public IEnumerator TypeText (MonoBehaviour caller, Text textField, string textToType, AudioSource audioSource, AudioClip typingClip) {
		if (textField == null)
			yield break;

		float originalPitch = 1;
		if (audioSource != null && typingClip != null) {
			originalPitch = audioSource.pitch;
		}

		_stringBuilder.Length = 0;
		//string message = "";
		for (int c = 0; c < textToType.Length; c++) {
			if (_abortTyping) {
				textField.text = textToType;
				break;
			}

			_stringBuilder.Append (textToType [c]);
			//message += textToType[c];
			textField.text = _stringBuilder.ToString ();
			//textField.text = message;

			if (_rushTyping) {
				yield return _waitZero;
			} else {
				if (audioSource != null && typingClip != null) {
					PlayRandomPitchClip (audioSource, typingClip);
				}
				yield return GetWaitTime (textToType [c]);
			}
		}

		if (audioSource != null && typingClip != null) {
			audioSource.pitch = originalPitch;
		}
		_rushTyping = false;
		_abortTyping = false;
		yield return null;
	}

	public IEnumerator TypeText (MonoBehaviour caller, Text textField, AudioSource audioSource, AudioClip typingClip) {
		if (textField == null)
			yield break;

		yield return (caller.StartCoroutine (TypeText (caller, textField, textField.text, audioSource, typingClip)));

	}

	public IEnumerator TypeText (MonoBehaviour caller, Text textField, string textToType) {
		if (textField == null)
			yield break;

		yield return (caller.StartCoroutine (TypeText (caller, textField, textToType, null, null)));

	}

	public IEnumerator TypeText (MonoBehaviour caller, Text textField) {
		if (textField == null)
			yield break;

		yield return (caller.StartCoroutine (TypeText (caller, textField, textField.text)));
	}

	private WaitForSeconds GetWaitTime (char c) {
		switch (c){
		case '.':
		case '!':
		case '?':
			return (_waitAfterPoint);
		case ',':
		case ':':
		case ';':
			return (_waitAfterComma);
		case ' ':
			return (_waitAfterSpace);
		default:
			return (_waitAfterChar);
		}
	}

	public void RushTyping () {
		if (!_rushTyping) {
			_rushTyping = true;
		} else {
			if (!_abortTyping) {
				_abortTyping = true;
			}
		}

	}

	public void AbortTyping () {
		_abortTyping = true;
	}

	private void PlayRandomPitchClip (AudioSource source, AudioClip clip) {
		source.pitch = Random.Range (_minPitch, _maxPitch);
		source.PlayOneShot (clip);
	}
	
}
