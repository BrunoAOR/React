using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TextMesh))]
public class Animated3DText : MonoBehaviour {

	public string	displayText = "Animated Text";
	public float	generalDelay = 0.0f;
	public float	lifeTimeAfterDelay = 1.0f;

	[Header ("Grow")]
	public 	bool	shouldGrow = true;
	public float	growthDelay = 0.0f;
	public float	growthDuration = 0.5f;
	public int		initialFontSize = 0;
	public int		finalFontSize = 100;

	[Header ("Color Faint")]
	public bool		shouldFaint = true;
	public float	faintDelay = 0.5f;
	public float	faintDuration = 0.5f;
	public Color	initialColor = Color.white;
	public Color	finalColor = Color.white;

	[Header ("Move")]
	public bool		shouldMove = true;
	public float	moveDelay = 0.0f;
	public float	moveDuration = 1.0f;
	public float	moveDistance = 0.5f;
	public Vector3	initialOffset = 1.5f * Vector3.up;
	public Vector3	moveDirection = Vector3.up;

	private Renderer	_renderer;
	private TextMesh	_textMesh;
	private float		_startTime;
	private float		_elapsedTime;
	private Vector3		_moveStartPosition;
	private Vector3		_moveEndPosition;


	void Awake () {
		_textMesh = GetComponent<TextMesh> ();
		_renderer = GetComponent<Renderer> ();
	}


	void Start () {
		_textMesh.text = displayText;
		_textMesh.fontSize = initialFontSize;
		_textMesh.color = initialColor;
		_renderer.enabled = false;

		_moveStartPosition = transform.position + initialOffset;
		_moveEndPosition = _moveStartPosition + moveDistance * moveDirection.normalized;

		transform.position = _moveStartPosition;
		transform.rotation = Quaternion.LookRotation (Camera.main.transform.forward, Camera.main.transform.up);

		_startTime = Time.time;
	}


	void Update () {
		transform.rotation = Quaternion.LookRotation (Camera.main.transform.forward, Camera.main.transform.up);

		_elapsedTime = Time.time - _startTime;

		if (_elapsedTime < generalDelay) {
			return;
		} else {
			_renderer.enabled = true;
			_elapsedTime -= generalDelay;
		}

		if (shouldGrow) {
			Grow ();
		}

		if (shouldFaint) {
			Faint ();
		}

		if (shouldMove) {
			Move ();
		}

		if (_elapsedTime >= lifeTimeAfterDelay) {
			Destroy (gameObject);
		}

	}


	private void Move () {
		if (_elapsedTime < moveDelay) {
			return;
		}

		float localTime = _elapsedTime - moveDelay;

		float u = localTime / moveDuration;

		if (u >= 1) {
			transform.position = _moveEndPosition;
		} else {
			transform.position = (1 - u) * _moveStartPosition + u * _moveEndPosition;
		}

	}


	private void Grow () {
		if (_elapsedTime < growthDelay) {
			return;
		}

		float localTime = _elapsedTime - growthDelay;

		float u = localTime / growthDuration;

		if (u >= 1) {
			_textMesh.fontSize = finalFontSize;
		} else {
			_textMesh.fontSize = (int) ((1 - u) * initialFontSize + u * finalFontSize);
		}
	}


	private void Faint () {
		if (_elapsedTime < faintDelay) {
			return;
		}

		float localTime = _elapsedTime - faintDelay;

		float u = localTime / faintDuration;

		if (u >= 1) {
			_textMesh.color = finalColor;;
		} else {
			_textMesh.color = (1 - u) * initialColor + u * finalColor;
		}
	}
}
