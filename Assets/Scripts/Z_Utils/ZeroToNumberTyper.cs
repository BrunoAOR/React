using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ZeroToNumberTyper {

	public static IEnumerator StartCounter (Text textField, int counterStart, int counterEnd, float duration) {
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
			yield return null;
		}

		textField.text = counterEnd.ToString ();
	}
}
