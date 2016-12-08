using UnityEngine;
using System.Collections;

public class LightRim : MonoBehaviour {

	[Header ("Alpha Animation")]
	public float cycleDuration = 0.5f;
	[Range (0,1)]
	public float alphaValue = 0.75f;

	[HideInInspector] public bool isLit;

	private SpriteRenderer _spriteRenderer;
	private IEnumerator _currentAlphaAnimation;
	private float alphaMultiplier = 1f;

	void Awake () {
		_spriteRenderer = GetComponent<SpriteRenderer> ();
		gameObject.SetActive (false);
		alphaMultiplier = 1f;
	}


	public Color GetLightColor () {
		Color tColor = _spriteRenderer.color;
		tColor.a = 1;
		return (tColor);
	}


	public void SetLightColor (Color newColor) {
		_spriteRenderer.color = newColor;
	}


	public void SetAlphaMultiplier (float newAlphaMultiplier) {
		alphaMultiplier = Mathf.Clamp01 (newAlphaMultiplier);
	}


	public void TurnLightOn () {
		isLit = true;
		gameObject.SetActive (true);

		if (_currentAlphaAnimation != null) {
			StopCoroutine (_currentAlphaAnimation);
			_currentAlphaAnimation = null;
		}
		_currentAlphaAnimation = AlphaAnimation ();
		StartCoroutine (_currentAlphaAnimation);
	}


	public void TurnLightOff () {
		isLit = false;
		gameObject.SetActive (false);
		if (_currentAlphaAnimation != null) {
			StopCoroutine (_currentAlphaAnimation);
			_currentAlphaAnimation = null;
		}
		Color tColor = _spriteRenderer.color;
		tColor.a = 1;
		_spriteRenderer.color = tColor;
	}


	private IEnumerator AlphaAnimation () {

		if (cycleDuration == 0)
			yield break;

		Color startColor = _spriteRenderer.color;
		Color finalColor = startColor;
		finalColor.a = alphaValue;

		while (isLit) {
			float startTime = Time.time;
			Color currentColor;
			// During the first half of the cycle, lower the alpha to alphaValue 
			while (Time.time - startTime < cycleDuration / 2) {
				float u = (Time.time - startTime) / (cycleDuration / 2);
				currentColor = Color.Lerp (startColor, finalColor, u);
				currentColor.a *= alphaMultiplier;
				_spriteRenderer.color = currentColor;
				yield return null;
			}
			// During the second half of the cycle, increase the alpha back to the original value
			while (Time.time - startTime < cycleDuration) {
				float u = (Time.time - startTime - cycleDuration / 2) / (cycleDuration / 2);
				currentColor = Color.Lerp (finalColor, startColor, u);
				currentColor.a *= alphaMultiplier;
				_spriteRenderer.color = currentColor;
				yield return null;
			}
		}

		_spriteRenderer.color = startColor;

	}


	private void OnValidate () {
		if (cycleDuration < 0) {
			cycleDuration = 0;
		}
	}
}
