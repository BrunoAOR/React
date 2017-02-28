using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZeroToNumberTyper {

	private float minPitch = 0.8f;
	private float maxPitch = 1.2f;
	private bool abort = false;

	public void SetAudioPitchParameters (float minimumPitch, float maximumPitch) {
		minPitch = minimumPitch;
		maxPitch = maximumPitch;
	}

	public IEnumerator StartCounter (Text textField, int counterStart, int counterEnd, float duration, AudioSource audioSource, AudioClip countingClip) {
		if (textField == null || counterStart > counterEnd || duration < 0) {
			yield break;
		}

		if (counterStart == counterEnd) {
			textField.text = counterStart.ToString ();
			yield break;
		}

		abort = false;

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
			if (abort) {
				yield break;
			}
		}

		textField.text = counterEnd.ToString ();
	}

	public IEnumerator StartCounter (Text textField, int counterStart, int counterEnd, float duration) {
		yield return (StartCounter (textField, counterStart, counterEnd, duration, null, null));
	}

	public void StopCounter () {
		abort = true;
	}

	private void PlayRandomPitchClip (AudioSource source, AudioClip clip) {
		source.pitch = Random.Range (minPitch, maxPitch);
		source.PlayOneShot (clip);
	}
}
