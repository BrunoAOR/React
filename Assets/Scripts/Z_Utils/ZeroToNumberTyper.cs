using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ZeroToNumberTyper {

	private static float minPitch = 0.8f;
	private static float maxPitch = 1.2f;

	public static void SetAudioPitchParameters (float minimumPitch, float maximumPitch) {
		minPitch = minimumPitch;
		maxPitch = maximumPitch;
	}

	public static IEnumerator StartCounter (Text textField, int counterStart, int counterEnd, float duration, AudioSource audioSource, AudioClip countingClip) {
		if (textField == null || counterStart > counterEnd || duration < 0) {
			yield break;
		}

		if (counterStart == counterEnd) {
			textField.text = counterStart.ToString ();
			yield break;
		}

		float startTime = Time.time;
		float u;
		float currentValue = counterStart;

		while ((Time.time - startTime) < duration) {
			u = (Time.time - startTime) / duration;
			currentValue = (1 - u) * counterStart + u * counterEnd;
			textField.text = ((int)currentValue).ToString ();
			if (audioSource != null && countingClip != null) {
				PlayRandomPitchClip (audioSource, countingClip);
			}
			yield return null;
		}

		textField.text = counterEnd.ToString ();
	}

	public static IEnumerator StartCounter (Text textField, int counterStart, int counterEnd, float duration) {
		yield return (StartCounter (textField, counterStart, counterEnd, duration, null, null));
	}

	private static void PlayRandomPitchClip (AudioSource source, AudioClip clip) {
		float pitch = Random.Range (minPitch, maxPitch);
		source.pitch = pitch;
		source.PlayOneShot (clip);
	}
}
