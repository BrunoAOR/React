using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class TextTyper {

	private static WaitForSeconds waitAfterChar = new WaitForSeconds (0.05f);
	private static WaitForSeconds waitAfterSpace = new WaitForSeconds (0.1f);
	private static WaitForSeconds waitAfterComma = new WaitForSeconds (0.1f);
	private static WaitForSeconds waitAfterPoint = new WaitForSeconds (0.5f);
	private static WaitForSeconds waitZero = new WaitForSeconds (0.001f);
	private static float minPitch = 0.8f;
	private static float maxPitch = 1.2f;
	private static bool rushTyping = false;
	private static bool abortTyping = false;

	public static void SetWaitParameters (float aWaitAfterChar, float aWaitAfterSpace, float aWaitAfterComma, float aWaitAfterPoint) {
		waitAfterChar = new WaitForSeconds (aWaitAfterChar);
		waitAfterSpace = new WaitForSeconds (aWaitAfterSpace);
		waitAfterComma = new WaitForSeconds (aWaitAfterComma);
		waitAfterPoint = new WaitForSeconds (aWaitAfterPoint);
	}

	public static void SetAudioPitchParameters (float minimumPitch, float maximumPitch) {
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
	public static IEnumerator TypeText (MonoBehaviour caller, Text textField, string textToType, AudioSource audioSource, AudioClip typingClip) {
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

	public static IEnumerator TypeText (MonoBehaviour caller, Text textField, AudioSource audioSource, AudioClip typingClip) {
		if (textField == null)
			yield break;

		string textToType = textField.text;

		yield return (caller.StartCoroutine (TypeText (caller, textField, textToType, audioSource, typingClip)));

	}

	public static IEnumerator TypeText (MonoBehaviour caller, Text textField, string textToType) {
		if (textField == null)
			yield break;

		yield return (caller.StartCoroutine (TypeText (caller, textField, textToType, null, null)));

	}

	public static IEnumerator TypeText (MonoBehaviour caller, Text textField) {
		if (textField == null)
			yield break;

		string textToType = textField.text;

		yield return (caller.StartCoroutine (TypeText (caller, textField, textToType)));
	}

	private static WaitForSeconds GetWaitTime (char c) {
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

	public static void RushTyping () {
		if (!rushTyping) {
			rushTyping = true;
		} else {
			if (!abortTyping) {
				abortTyping = true;
			}
		}

	}

	public static void AbortTyping () {
		abortTyping = true;
	}

	private static void PlayRandomPitchClip (AudioSource source, AudioClip clip) {
		float pitch = Random.Range (minPitch, maxPitch);
		source.pitch = pitch;
		source.PlayOneShot (clip);
	}
	
}
