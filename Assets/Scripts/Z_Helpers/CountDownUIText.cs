using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (RectTransform))]
[RequireComponent (typeof (CanvasRenderer))]
[RequireComponent (typeof (Text))]
public class CountDownUIText : MonoBehaviour {

	[Header ("Time Parameters")]
	public float localTimeScale = 1f;

	[Header ("Count Down")]
	//public Text countDownText;
	public Color countDownTextColor;
	public int countStart = 3;

	[Header ("Blending the Zero")]
	public bool shouldZeroBlend = true;
	public bool shouldYieldForZeroBlend = false;
	[Range (0,1)]
	public float zeroBlendTime = 0.5f;

	[Header ("Scaling")]
	public int startSize = 250;
	public int endSize = 25;

	public bool isCountingDown {
		get {
			return _isCountingDown;
		}
	}
	private bool _isCountingDown;

	public bool isPaused {
		get {
			return _isPaused;
		}
	}
	private bool _isPaused;

	// Local Time
	private float localTime = 0;

	// References
	private Text _textField;
	private IEnumerator currentCountDown;
	private IEnumerator currentZeroBlend;


	void Awake () {
		_textField = GetComponent<Text> ();
	}


	void Start () {
		localTime = 0;
		_textField.horizontalOverflow = HorizontalWrapMode.Overflow;
		_textField.verticalOverflow = VerticalWrapMode.Overflow;
		_textField.text = "";
		_textField.alignment = TextAnchor.MiddleCenter;
	}


	void Update () {
		if (_isCountingDown && !_isPaused) {
			localTime += Time.deltaTime * localTimeScale;
		}
	}


	public IEnumerator StartCountDown () {
		StopCountDown ();	// StopCountDown will check if there is a CountDown being executed.
		gameObject.SetActive (true);
		currentCountDown = CountDown ();
		yield return null;
		yield return (StartCoroutine (currentCountDown));
		currentCountDown = null;
		if (!shouldZeroBlend) {
			gameObject.SetActive (false);
		}
	}


	public void StopCountDown () {
		if (currentCountDown != null) {
			StopCoroutine (currentCountDown);
			currentCountDown = null;
			_isCountingDown = false;
		}
		if (currentZeroBlend != null) {
			StopCoroutine (currentZeroBlend);
			currentZeroBlend = null;
		}
		gameObject.SetActive (false);
	}


	public void PauseCountDown () {
		_isPaused = true;
	}


	public void UnPauseCountDown () {
		_isPaused = false;
	}


	private IEnumerator CountDown () {
		_isCountingDown = true;
		float timeStart;

		_textField.fontSize = startSize;
		_textField.color = countDownTextColor;

		for (int i = countStart; i > 0; i--) {
			_textField.text = i.ToString ("D");

			timeStart = localTime;
			while (localTime - timeStart < 1) {

				while (isPaused)
					yield return null;
				
				float u = (localTime - timeStart);
				float textSize = (1 - u) * startSize + u * endSize;
				_textField.fontSize = (int)textSize;
				yield return null;
			}
		}

		_textField.text = "";
		_textField.color = countDownTextColor;

		if (shouldZeroBlend) {
			if (shouldYieldForZeroBlend) {
				yield return (StartCoroutine (ZeroBlendHandler ()));
			} else {
				StartZeroBlend ();
			}
		} else {
			_isCountingDown = false;
		}
	}


	private void StartZeroBlend () {
		StartCoroutine (ZeroBlendHandler ());
	}


	private IEnumerator ZeroBlendHandler () {
		currentZeroBlend = ZeroBlend ();
		yield return (StartCoroutine (currentZeroBlend) );
		currentZeroBlend = null;

		_isCountingDown = false;
		gameObject.SetActive (false);
	}


	private IEnumerator ZeroBlend () {
		float timeStart = localTime;

		_textField.text = "0";

		_textField.fontSize = startSize;
		Color startColor = countDownTextColor;
		Color endColor = startColor;
		endColor.a = 0;

		while (localTime - timeStart < zeroBlendTime) {

			while (isPaused)
				yield return null;

			float u = (localTime - timeStart) / zeroBlendTime;
			_textField.color = Color.Lerp (startColor, endColor, u);
			yield return null;
		}
	}

}
