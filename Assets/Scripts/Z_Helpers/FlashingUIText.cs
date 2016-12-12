using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Text))]
public class FlashingUIText : MonoBehaviour {

	public bool flashOnEnable = true;
	public Color dimColor = new Color (1f, 1f, 1f, 0.25f);
	public Color highlightColor = new Color (1f, 1f, 1f, 1f);
	public float dimScale = 0.9f;
	public float highlightScale = 1.0f;
	public float cycleDuration = 0.5f;

	private bool _setupDone = false;
	private bool _flashing = false;
	private Text _text;
	private Color _startColor;
	private Vector3 _startScale;
	private float _startTime;
	private float _cycleTime;
	private float _u;
	private Color _currentColor;
	private Vector3 _currentScale;
	private float _halfCycle {
		get {
			return (cycleDuration / 2);
		}
	}

	void Awake () {
		Setup ();
	}


	private void Setup () {
		if (_setupDone)
			return;

		_text = GetComponent<Text> ();
		_startColor = _text.color;
		_startScale = _text.gameObject.transform.localScale;
		_setupDone = true;
	}


	void OnEnable () {
		if (flashOnEnable)
			Flash ();
	}


	void OnDisable () {
		_flashing = false;
		_text.color = _startColor;
		_text.gameObject.transform.localScale = _startScale;
	}


	private void Flash () {
		Setup ();
		_flashing = true;
		_startTime = Time.time;
	}


	void Update () {
		if (!_flashing)
			return;

		_cycleTime = (Time.time - _startTime) % cycleDuration;

		if (_cycleTime <= _halfCycle) {
			// HIGHLIGHTING
			_u = _cycleTime / _halfCycle;
			_currentColor = Color.Lerp (dimColor, highlightColor, _u);
			_currentScale = Vector3.one * ((1 - _u) * dimScale + _u * highlightScale);
		} else {
			// DIMMING
			_u = (_cycleTime - _halfCycle) / _halfCycle;
			_currentColor = Color.Lerp (highlightColor, dimColor, _u);
			_currentScale = Vector3.one * ((1 - _u) * highlightScale + _u * dimScale);
		}

		_text.color = _currentColor;
		_text.gameObject.transform.localScale = _currentScale;
	}


	public void SetText (string message) {
		Setup ();

		_text.text = message;
	}


	public void SetFontSize (int fontSize) {
		Setup ();

		if (fontSize < 0)
			fontSize = 0;
		
		_text.fontSize = fontSize;
	}
}
