using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BGLineSpawner : MonoBehaviour {

	public GameObject linePrefab;
	public GameObjectPool lineObjectPool;

	public bool shouldSpawn = true;
	public int spawnRate = 5;

	[Range (0.4f, 1f)]
	public float lineLengthMin = 0.5f;
	[Range (0.8f, 2f)]
	public float lineLengthMax = 1.0f;

	public float minSpeed = 0.5f;
	public float maxSpeed = 1.0f;

	public float alphaChangeSpeed = 1f;

	private CanvasScaler _canvasScaler;
	private CanvasGroup _canvasGroup;
	private float _screenWidthPx;
	private float _screenHeightPx;
	private float _lengthMin;
	private float _lengthMax;
	private float _timeWaited;

	// Use this for initialization
	void Awake () {
		_canvasScaler = GetComponent <CanvasScaler> ();
		_canvasGroup = GetComponent <CanvasGroup> ();

		if (lineObjectPool == null)
			enabled = false;
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
			GameObject tGO = lineObjectPool.GetGameObject ();
			tGO.transform.SetParent (transform);
			tGO.transform.localScale = Vector3.one;

			// Get a reference to the BGLineMover (which contains references to the RectTransform and the Image)
			BGLineMover tBGLineMover = tGO.GetComponent<BGLineMover> ();

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

			if (random < 1) {
				// UP
				tBGLineMover.rectTransform.position = new Vector2 (Random.Range(0, _screenWidthPx), _screenHeightPx + scalableLineLength / 2);
				tBGLineMover.rectTransform.rotation = Quaternion.Euler (0, 0, 270);
				tBGLineMover.lifeTime = (scalableLineLength + _screenHeightPx) / (lineSpeed * _screenWidthPx);
			} else if (random < 2) {
				// DOWN
				tBGLineMover.rectTransform.position = new Vector2 (Random.Range(0, _screenWidthPx), 0 - scalableLineLength / 2);
				tBGLineMover.rectTransform.rotation = Quaternion.Euler (0, 0, 90);
				tBGLineMover.lifeTime = (scalableLineLength + _screenHeightPx) / (lineSpeed * _screenWidthPx);
			} else if (random < 3) {
				// LEFT
				tBGLineMover.rectTransform.position = new Vector2 (0 - scalableLineLength / 2, Random.Range(0, _screenHeightPx));
				tBGLineMover.rectTransform.rotation = Quaternion.Euler (0, 0, 0);
				tBGLineMover.lifeTime = (scalableLineLength + _screenWidthPx) / (lineSpeed * _screenWidthPx);
			} else {	// random <4
				// RIGHT
				tBGLineMover.rectTransform.position = new Vector2 (_screenWidthPx + scalableLineLength / 2, Random.Range(0,_screenHeightPx));
				tBGLineMover.rectTransform.rotation = Quaternion.Euler (0, 0, 180);
				tBGLineMover.lifeTime = (scalableLineLength + _screenWidthPx) / (lineSpeed * _screenWidthPx);
			}

			tBGLineMover.ReturnToPoolAfterLifeTime ();

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
