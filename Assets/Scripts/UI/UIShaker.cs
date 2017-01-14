using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShaker : MonoBehaviour {

	[Header ("Shake Rotate")]
	public bool shakeRotateOnAwake = false;
	public float shakeAngleAmplitude = 15f;
	public float shakeCycleDuration = 0.25f;
	public int shakeCycleCount = 3;

	private Quaternion _initialRotation;
	private IEnumerator _currentShakeRotate;

	private RectTransform _rTransform;

	void Awake () {
		_rTransform = GetComponent<RectTransform> ();
		if (shakeRotateOnAwake) {
			StartShakeRotate ();
		}
	}

	public void StartShakeRotate () {
		if (_rTransform == null) {
			Awake ();
		}

		if (_currentShakeRotate != null) {
			StopShakeRotate ();
		}

		_initialRotation = _rTransform.localRotation;
		_currentShakeRotate = ShakeRotate ();
		StartCoroutine (_currentShakeRotate);
	}

	public void StopShakeRotate () {
		if (_currentShakeRotate != null) {
			StopCoroutine (_currentShakeRotate);
			_currentShakeRotate = null;
			_rTransform.localRotation = _initialRotation;
		}
	}

	private IEnumerator ShakeRotate () {

		// Clockwise = -z
		float subCycleDuration = shakeCycleDuration /4f;
		float zeroZRotation = _initialRotation.z;
		float maxZRotation = zeroZRotation + shakeAngleAmplitude;
		float minZRotation = zeroZRotation - shakeAngleAmplitude;

		Vector3 currentRotation = _initialRotation.eulerAngles;
		float subCycleStart;
		float u;

		for (int cycle = 0; cycle < shakeCycleCount; cycle++) {
			// First 1/4 cycle: From zeroZRotation to maxZRotation
			subCycleStart = Time.time;
			while ( (Time.time - subCycleStart) < subCycleDuration ) {
				u = (Time.time - subCycleStart) / subCycleDuration;
				currentRotation.z = (1 - u) * zeroZRotation + u * maxZRotation;
				_rTransform.localRotation = Quaternion.Euler (currentRotation);
				yield return null;
			}

			// Next 1/2 cycle: From maxZRotation to minZRotation
			subCycleStart = Time.time;
			while ( (Time.time - subCycleStart) < (2 * subCycleDuration) ) {
				u = (Time.time - subCycleStart) / (2 * subCycleDuration);
				currentRotation.z = (1 - u) * maxZRotation + u * minZRotation;
				_rTransform.localRotation = Quaternion.Euler (currentRotation);
				yield return null;
			}

			// Last 1/4 cycle: From minZRotation to zeroZRotation
			subCycleStart = Time.time;
			while ( (Time.time - subCycleStart) < subCycleDuration ) {
				u = (Time.time - subCycleStart) / subCycleDuration;
				currentRotation.z = (1 - u) * minZRotation + u * zeroZRotation;
				_rTransform.localRotation = Quaternion.Euler (currentRotation);
				yield return null;
			}

		}

		_rTransform.localRotation = _initialRotation;
		_currentShakeRotate = null;
	}
}
