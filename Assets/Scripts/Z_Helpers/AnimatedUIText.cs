using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (RectTransform))]
[RequireComponent (typeof (Text))]
public class AnimatedUIText : MonoBehaviour {

	[Header ("Time Range (for free animation)")]
	public float timeStart = 1f;
	public float timeEnd = 0f;

	private float freeTimer;

	[Header ("General")]
	public float generalDelay;
	public bool shouldAnimateOnTime = true;
	public float lifeTimeAfterDelay = 1.0f;

	private bool animatingOnTime = false;

	[Header ("Anchor Moving")]
	public bool shouldMove = true;
	public float moveDelay = 0f;
	public float moveDuration = 1f;
	public Vector2 startAnchor;
	public Vector2 targetAnchor;

	private Vector2 currentAnchor;

	[Header ("Font Scaling")]
	public bool shouldScale = true;
	public float scaleDelay = 0f;
	public float scaleDuration = 1f;
	public int startFontSize;
	public int targetFontSize;

	[Header ("Text Coloring")]
	public bool shouldColor = true;
	public float colorDelay = 0f;
	public float colorDuration = 1f;
	public Color startColor;
	public Color targetColor;

	[Header ("Text Shaking (Z-Rotation)")]
	public bool shouldShake = false;
	public float shakeDelay = 0f;
	public float shakeDuration = 1f;
	public float shakeCycle = 0.1f;
	public float shakeAmplitude = 10f;

	private Quaternion _referenceRotation;
	private Quaternion _currentRotation;
	private float _refZRot;

	// General references
	private RectTransform _rectTransform;
	private Text _text;

	// For animatingOnTime
	private float _startTime;
	private float _elapsedTime;

	// For free animation
	private float _freeElapsedTime;


	void Awake () {
		_rectTransform = GetComponent<RectTransform> ();
		_text = GetComponent<Text> ();
		_referenceRotation = _rectTransform.rotation;
		_currentRotation = _referenceRotation;
		_refZRot = _referenceRotation.eulerAngles.z;
	}


	void Start () {
		if (shouldAnimateOnTime) {
			animatingOnTime = true;
			_startTime = Time.time;
		}
	}


	void OnEnable () {
		_rectTransform.rotation = _referenceRotation;
	}


	void Update () {
		if (!animatingOnTime)
			return;

		// So, animatingOnTime == true;

		_elapsedTime = Time.time - _startTime;

		if (_elapsedTime < generalDelay) {
			return;
		} else {
			_elapsedTime -= generalDelay;
		}

		Animate ();

		if (_elapsedTime >= lifeTimeAfterDelay) {
			Destroy (gameObject);
		}
	}


	public void ApplyStartConditions () {

		if (!animatingOnTime)
			_text.text = timeStart.ToString ("F1");

		_rectTransform.anchorMin = startAnchor;
		_rectTransform.anchorMax = startAnchor;
		_text.fontSize = startFontSize;
		_text.color = startColor;
	}


	public void SetDisplayText (string message) {
		_text.text = message;
	}

	public void AdjustFreeTimeScale (float timeStart, float timeEnd = 0f) {
		float newRange = Mathf.Abs (timeStart - timeEnd);
		float oldRange = Mathf.Abs (this.timeStart - this.timeEnd);

		float timeScale = newRange / oldRange;

		// Adjust time parameters
		generalDelay *= timeScale;
		this.timeStart = timeStart;
		this.timeEnd = timeEnd;

		// Reset text to new start value
		_text.text = timeStart.ToString ("F1");

		// Adjust move parameters
		moveDelay *= timeScale;
		moveDuration *= timeScale;

		// Adjust scale parameters
		scaleDelay *= timeScale;
		scaleDuration *= timeScale;

		// Adjust color parameters
		colorDelay *= timeScale;
		colorDuration *= timeScale;

		// Adjust shake parameters
		shakeDelay *= timeScale;
		shakeDuration *= timeScale;
	}


	public void SetTime (float time) {
		if (animatingOnTime)
			return;

		// So, animatingOnTime == false; ==> Free animation

		_text.text = time.ToString ("F1");

		freeTimer = time;
		_freeElapsedTime = Mathf.Abs(freeTimer - timeStart);

		if (_freeElapsedTime < generalDelay) {
			return;
		} else {
			_freeElapsedTime -= generalDelay;
		}
		
		Animate ();
	}


	void Animate () {
		if (shouldMove)
			DoMove ();

		if (shouldScale)
			DoScale ();

		if (shouldColor)
			DoColor ();

		if (shouldShake)
			DoShake ();
	}


	void DoMove () {
		float selectedElapsedTime = animatingOnTime ? _elapsedTime : _freeElapsedTime;

		if (selectedElapsedTime < moveDelay)
			return;

		float localTime = selectedElapsedTime - moveDelay;

		float u = localTime / moveDuration;

		if (u >= 1) {
			_rectTransform.anchorMin = targetAnchor;
			_rectTransform.anchorMax = targetAnchor;
		} else {
			currentAnchor = Vector2.LerpUnclamped (startAnchor, targetAnchor, u);
			_rectTransform.anchorMin = currentAnchor;
			_rectTransform.anchorMax = currentAnchor;
		}
	}


	void DoScale () {
		float selectedElapsedTime = animatingOnTime ? _elapsedTime : _freeElapsedTime;

		if (selectedElapsedTime < scaleDelay)
			return;

		float localTime = selectedElapsedTime - scaleDelay;

		float u = localTime / scaleDuration;

		if (u >= 1) {
			_text.fontSize = targetFontSize;
		} else {
			_text.fontSize = (int) ((1 - u) * startFontSize + u * targetFontSize);
		}
	}


	void DoColor () {
		float selectedElapsedTime = animatingOnTime ? _elapsedTime : _freeElapsedTime;

		if (selectedElapsedTime < colorDelay)
			return;

		float localTime = selectedElapsedTime - colorDelay;

		float u = localTime / colorDuration;

		if (u >= 1) {
			_text.color = targetColor;
		} else {
			_text.color = Color.LerpUnclamped (startColor, targetColor, u);
		}
	}


	void DoShake () {
		if (shakeCycle == 0)
			return;

		float selectedElapsedTime = animatingOnTime ? _elapsedTime : _freeElapsedTime;

		if (selectedElapsedTime < shakeDelay)
			return;

		float localTime = selectedElapsedTime - shakeDelay;

		float u = localTime / shakeDuration;
		if (u >= 1) {
			_rectTransform.rotation = _referenceRotation;
		} else {
			float u2 = (localTime % (shakeCycle / 4)) / (shakeCycle / 4);
			float zRot = 0;
			if (u2 < 0.25f) { // Going mid->left (refZ->maxZ)
				zRot = (1-u2) * _refZRot + u2 * (_refZRot + shakeAmplitude);
			} else if (u2 < 0.75f) { // Going left->right (maxZ->minZ)
				zRot = (1-u2) * (_refZRot + shakeAmplitude) + u2 * (_refZRot - shakeAmplitude);
			} else { // Going right->mid (minZ->refZ)
				zRot = (1-u2) * (_refZRot - shakeAmplitude) + u2 * _refZRot;
			}
			Vector3 eulerAngles = _currentRotation.eulerAngles;
			eulerAngles.z = zRot;
			_currentRotation.eulerAngles = eulerAngles;
			_rectTransform.rotation = _currentRotation;
		}
	}


}