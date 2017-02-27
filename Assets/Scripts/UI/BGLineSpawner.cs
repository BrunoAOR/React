using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BGLineSpawner : MonoBehaviour {

	public BGLineMover linePrefab;
	public Transform poolTransform;

	public bool shouldSpawn = true;
	public int spawnRate = 5;

	[Range (0.4f, 1f)]
	public float lineLengthMin = 0.5f;
	[Range (0.8f, 2f)]
	public float lineLengthMax = 1.0f;

	public float minSpeed = 0.5f;
	public float maxSpeed = 1.0f;

	public float alphaChangeSpeed = 1f;

	private ComponentPool<BGLineMover> lineMoverPool;
	private CanvasScaler _canvasScaler;
	private CanvasGroup _canvasGroup;
	private float _screenWidthPx;
	private float _screenHeightPx;
	private float _lengthMin;
	private float _lengthMax;
	private float _timeWaited;

	// Use this for initialization
	void Awake () {
		_canvasScaler = GetComponentInParent <CanvasScaler> ();
		_canvasGroup = GetComponent <CanvasGroup> ();

		if (linePrefab == null) {
			enabled = false;
			return;
		}
		lineMoverPool = new ComponentPool<BGLineMover> (linePrefab, poolTransform);
	}


	void OnEnable () {
		_timeWaited = 0;
	}


	void Start () {
		_screenWidthPx = Camera.main.pixelWidth;
		_screenHeightPx = Camera.main.pixelHeight;
		_lengthMin = lineLengthMin * _canvasScaler.referenceResolution.x;
		_lengthMax = lineLengthMax * _canvasScaler.referenceResolution.x;
	}


	public void SetAlpha (float alphaValue) {
		float targetAlpha = Mathf.Clamp (alphaValue, 0f, 1f);
		if (targetAlpha != GetAlpha())
			StartCoroutine (SetAlphaOvertime (targetAlpha));
	}


	private IEnumerator SetAlphaOvertime (float targetAlpha) {
		float startTime = Time.time;
		float startAlpha = GetAlpha ();
		float duration = (targetAlpha - startAlpha) / alphaChangeSpeed;
		float u;
		float currentAlpha;

		while ((Time.time - startTime) < duration) {
			u = (Time.time - startTime) / duration;
			currentAlpha = (1 - u) * startAlpha + u * targetAlpha;
			_canvasGroup.alpha = currentAlpha;
			yield return null;
		}

		_canvasGroup.alpha = targetAlpha;

	}


	public float GetAlpha () {
		return (_canvasGroup.alpha);
	}


	void Update () {
		_timeWaited += Time.deltaTime;

		// Determine if a new line should be spawned
		if (_timeWaited >= 1f / spawnRate) {
			// Spawn new line
			_timeWaited = 0;
			BGLineMover tBGLineMover = lineMoverPool.Spawn ();
			tBGLineMover.transform.SetParent (transform, false);

			// Assign a random line length
			float lineLength = Random.Range (_lengthMin, _lengthMax);
			tBGLineMover.rectTransform.sizeDelta = new Vector2 (lineLength, tBGLineMover.rectTransform.sizeDelta.y);

			// Assign a random color (use HSV and value of 1)
			tBGLineMover.image.color = Random.ColorHSV (0f, 1f, 0f, 1f, 1f, 1f, 1f, 1f);

			// Assign a random speed
			float lineSpeed = Random.Range (minSpeed, maxSpeed);
			tBGLineMover.speed = lineSpeed;

			// Determine where to spawn it (up, down, left or right side of screen), angle it accordingly and determine lifetime
			float random = 4 * Random.value;
			float scalableLineLength = lineLength / _canvasScaler.referenceResolution.x * _screenWidthPx;

			float lifeTime = 0;
			if (random < 1) {
				// UP
				tBGLineMover.rectTransform.position = new Vector2 (Random.Range(0, _screenWidthPx), _screenHeightPx + scalableLineLength / 2);
				tBGLineMover.rectTransform.rotation = Quaternion.Euler (0, 0, 270);
				lifeTime = (scalableLineLength + _screenHeightPx) / (lineSpeed * _screenWidthPx);
			} else if (random < 2) {
				// DOWN
				tBGLineMover.rectTransform.position = new Vector2 (Random.Range(0, _screenWidthPx), 0 - scalableLineLength / 2);
				tBGLineMover.rectTransform.rotation = Quaternion.Euler (0, 0, 90);
				lifeTime = (scalableLineLength + _screenHeightPx) / (lineSpeed * _screenWidthPx);
			} else if (random < 3) {
				// LEFT
				tBGLineMover.rectTransform.position = new Vector2 (0 - scalableLineLength / 2, Random.Range(0, _screenHeightPx));
				tBGLineMover.rectTransform.rotation = Quaternion.Euler (0, 0, 0);
				lifeTime = (scalableLineLength + _screenWidthPx) / (lineSpeed * _screenWidthPx);
			} else {	// random <4
				// RIGHT
				tBGLineMover.rectTransform.position = new Vector2 (_screenWidthPx + scalableLineLength / 2, Random.Range(0,_screenHeightPx));
				lifeTime = (scalableLineLength + _screenWidthPx) / (lineSpeed * _screenWidthPx);
			}
			tBGLineMover.ReturnToPoolAfter (lifeTime);
		}
	}


	void OnValidate () {
		if (spawnRate < 0)
			spawnRate = 0;

		if (lineLengthMax < lineLengthMin + 0.4f)
			lineLengthMax = lineLengthMin + 0.4f;

		if (maxSpeed < minSpeed)
			maxSpeed = minSpeed;

		if (alphaChangeSpeed < 0)
			alphaChangeSpeed = 0;
	}

}
