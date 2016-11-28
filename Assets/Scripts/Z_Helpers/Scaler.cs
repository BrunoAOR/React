using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour {
	public float startScale = 1f;
	public float targetScale = 2f;
	public float scaleTime = 1f;

	private Vector3 referenceScale = Vector3.one;


	public void SnapToStartScale () {
		transform.localScale = startScale * referenceScale;
	}


	public void ScaleToTarget () {
		StartCoroutine (ScaleToTargetCoroutine ());
	}


	public IEnumerator ScaleToTargetCoroutine () {

		if (scaleTime == 0)
			yield break;

		Vector3 startScale = this.startScale * referenceScale;
		Vector3 endScale = targetScale * referenceScale;

		float startTime = Time.time;
		float u;

		Vector3 currentScale;

		while ((Time.time - startTime) <= scaleTime) {
			u = (Time.time - startTime) / scaleTime;
			currentScale = Vector3.LerpUnclamped (startScale, endScale, u);
			transform.localScale = currentScale;
			yield return null;
		}

		transform.localScale = endScale;
	}
}
