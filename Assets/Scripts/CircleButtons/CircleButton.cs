using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CircleButton : MonoBehaviour {

	[HideInInspector] public bool isLit;

	[Header ("Touch Animation")]
	public float animationDuration = 0.2f;
	[Range (0,1)]
	public float highlightStartPercent = 0.2f;
	[Range (0,1)]
	public float highlightEndPercent = 0.3f;
	[Range (0, 1.5f)]
	public float highlightScale = 0.5f;
	private Color highlightColor = Color.white;

	private LightRim _lightRim;

	// For animation
	[HideInInspector] public bool isAnimating {
		get {
			return _animating;
		}
	}
	private bool _animating;

	// General
	private SpriteRenderer _spriteRenderer;

	void Awake () {
		_spriteRenderer = GetComponent<SpriteRenderer> ();
		_lightRim = GetComponentInChildren<LightRim> ();
	}


	void OnMouseDown () {
		
		RoundManager.S.ButtonPressed (this);
	}


	public Color GetLightColor () {
		return _lightRim.GetLightColor ();
	}


	public void SetLightColor (Color newColor) {
		_lightRim.SetLightColor (newColor);
	}


	public void SetLightAlphaMultiplier (float alphaMultiplier) {
		alphaMultiplier = Mathf.Clamp01 (alphaMultiplier);
		_lightRim.SetAlphaMultiplier (alphaMultiplier);
	}


	public Color GetButtonColor () {
		return _spriteRenderer.color;
	}


	public 	void SetButtonColor (Color newColor) {
		_spriteRenderer.color = newColor;
	}


	public void TurnLightOn () {
		if (!isLit) {
			isLit = true;
			_lightRim.TurnLightOn ();
		}
	}


	public void TurnLightOff () {
		if (isLit) {
			isLit = false;
			_lightRim.TurnLightOff ();

			highlightColor = _lightRim.GetLightColor ();
			if (!_animating) {
				StartCoroutine (AnimateTouch ());
			}
		}
	}


	public void ForceLightOff () {
		if (isLit) {
			isLit = false;
			_lightRim.TurnLightOff ();
		}
	}


	public void SpawnGoodTimeBonus (float timeBonus) {
		Vector2 buttonPositionInViewport = Camera.main.WorldToViewportPoint (transform.position);
		PointsSpawner.S.AnimateGoodTimeBonus (Utils.GetSignedStringFromNumber (timeBonus), buttonPositionInViewport);
	}


	public void SpawnBadTimeBonus (float timeBonus) {
		Vector2 buttonPositionInViewport = Camera.main.WorldToViewportPoint (transform.position);
		PointsSpawner.S.AnimateBadTimeBonus (Utils.GetSignedStringFromNumber (timeBonus), buttonPositionInViewport);
		
	}


	private IEnumerator AnimateTouch () {
		_animating = true;

		Vector3 initialScale = transform.localScale;
		Color initialColor = _spriteRenderer.color;

		Vector3 highlightScale = this.highlightScale * Vector3.one;
		highlightScale.z = 1;

		float animationStartTime = Time.time;

		yield return null;

		// During the first section of the animation (highlighting), the highlightColor and highLightScale are achieved
		while (Time.time - animationStartTime < highlightStartPercent * animationDuration) {
			float u = (Time.time - animationStartTime) / (highlightStartPercent * animationDuration);
			_spriteRenderer.color = Color.Lerp (initialColor, highlightColor, u);
			transform.localScale = Vector3.Lerp (initialScale, highlightScale, u);
			yield return null;
		}

		// During the second section of the animation (waiting), the highlightColor and highLightScale are kept
		yield return new WaitForSeconds ( (highlightEndPercent - highlightStartPercent) * animationDuration);

		// During the third section of the animation (highlighting), the initialColor and initialScale are achieved
		while (Time.time - animationStartTime < animationDuration) {
			float u = (Time.time - (animationStartTime + highlightEndPercent * animationDuration)) / (animationDuration - highlightEndPercent * animationDuration);
			_spriteRenderer.color = Color.Lerp (highlightColor, initialColor, u);
			transform.localScale = Vector3.Lerp (highlightScale, initialScale, u);
			yield return null;
		}

		_spriteRenderer.color = initialColor;
		transform.localScale = initialScale;

		_animating = false;
	}


	private void OnValidate () {
		if (animationDuration < 0) {
			animationDuration = 0;
		}
		if (highlightEndPercent < highlightStartPercent) {
			highlightEndPercent = highlightStartPercent;
		}
	}

}
