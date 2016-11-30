using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour {
	public float duration = 0.5f;
	public float magnitude = 0.02f;
	public float perlinSeedMultiplier = 100f;

	private Transform _cameraTransform;
	private Vector3 _referencePositions;
	private IEnumerator currentShakeCoroutine;

	void Awake () {
		_cameraTransform = Camera.main.transform;
		_referencePositions = Camera.main.transform.position;
	}

	public void Shake () {
		if (currentShakeCoroutine != null) {
			StopCoroutine (currentShakeCoroutine);
			currentShakeCoroutine = null;
		}

		_cameraTransform.position = _referencePositions;

		currentShakeCoroutine = ShakeCoroutine ();
		StartCoroutine (currentShakeCoroutine);
	}

	private IEnumerator ShakeCoroutine () {

		float elapsedTime = 0f;

		while (elapsedTime < duration) {

			elapsedTime += Time.deltaTime;

			float percentComplete = elapsedTime / duration;
			// Damper is meant to be applied for the last 25% percent (3/4) of the animation
			float damper = 1.0f - Mathf.Clamp (4.0f * percentComplete -3.0f, 0.0f, 1.0f);

			float perlinSeed = Time.time * perlinSeedMultiplier;

			// Get random values for x and y in the range [-1, 1]
			float x = 2.0f * Mathf.PerlinNoise (perlinSeed, 0f) - 1.0f;
			float y = 2.0f * Mathf.PerlinNoise (0f, perlinSeed) - 1.0f;

			x *= magnitude * damper;
			y *= magnitude * damper;

			_cameraTransform.position = new Vector3 (x, y, _referencePositions.z);

			yield return null;
		}

		_cameraTransform.position = _referencePositions;

		currentShakeCoroutine = null;
	}

}
