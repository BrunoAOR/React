using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper {

	private WaitForSeconds waitAfterChar = new WaitForSeconds (0.05f);
	private WaitForSeconds waitAfterSpace = new WaitForSeconds (0.1f);
	private WaitForSeconds waitAfterComma = new WaitForSeconds (0.1f);
	private WaitForSeconds waitAfterPoint = new WaitForSeconds (0.5f);
	private WaitForSeconds waitZero = new WaitForSeconds (0.001f);
	private float minPitch = 0.8f;
	private float maxPitch = 1.2f;
	private bool rushTyping = false;
	private bool abortTyping = false;

	public void SetWaitParameters (float aWaitAfterChar, float aWaitAfterSpace, float aWaitAfterComma, float aWaitAfterPoint) {
		waitAfterChar = new WaitForSeconds (aWaitAfterChar);
		waitAfterSpace = new WaitForSeconds (aWaitAfterSpace);
		waitAfterComma = new WaitForSeconds (aWaitAfterComma);
		waitAfterPoint = new WaitForSeconds (aWaitAfterPoint);
	}

	public void SetAudioPitchParameters (float minimumPitch, float maximumPitch) {
		minPitch = minimumPitch;
		maxPitch = maximumPitch;
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

		string message = "";
		for (int c = 0; c < textToType.Length; c++) {
			if (abortTyping) {
				message = textToType;
				textField.text = message;
				break;
			}

			message += textToType[c];
			textField.text = message;

			if (rushTyping) {
				yield return waitZero;
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
		rushTyping = false;
		abortTyping = false;
		yield return null;
	}

	public IEnumerator TypeText (MonoBehaviour caller, Text textField, AudioSource audioSource, AudioClip typingClip) {
		if (textField == null)
			yield break;

		string textToType = textField.text;

		yield return (caller.StartCoroutine (TypeText (caller, textField, textToType, audioSource, typingClip)));

	}

	public IEnumerator TypeText (MonoBehaviour caller, Text textField, string textToType) {
		if (textField == null)
			yield break;

		yield return (caller.StartCoroutine (TypeText (caller, textField, textToType, null, null)));

	}

	public IEnumerator TypeText (MonoBehaviour caller, Text textField) {
		if (textField == null)
			yield break;

		string textToType = textField.text;

		yield return (caller.StartCoroutine (TypeText (caller, textField, textToType)));
	}

	private WaitForSeconds GetWaitTime (char c) {
		WaitForSeconds waitTime;
		switch (c){
		case '.':
		case '!':
		case '?':
			waitTime = waitAfterPoint;
			break;
		case ',':
		case ':':
		case ';':
			waitTime = waitAfterComma;
			break;
		case ' ':
			waitTime = waitAfterSpace;
			break;
		default:
			waitTime = waitAfterChar;
			break;
		}

		return waitTime;
	}

	public void RushTyping () {
		if (!rushTyping) {
			rushTyping = true;
		} else {
			if (!abortTyping) {
				abortTyping = true;
			}
		}

	}

	public void AbortTyping () {
		abortTyping = true;
	}

	private void PlayRandomPitchClip (AudioSource source, AudioClip clip) {
		float pitch = Random.Range (minPitch, maxPitch);
		source.pitch = pitch;
		source.PlayOneShot (clip);
	}
	
}
