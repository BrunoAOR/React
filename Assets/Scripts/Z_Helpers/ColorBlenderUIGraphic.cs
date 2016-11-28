using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorBlenderUIGraphic : MonoBehaviour {

	[Header ("Color Blending")]
	public Color alternateColor = Color.black;
	public Color startColor = Color.white;
	public Color endColor = Color.clear;

	[Header ("Time Parameters")]
	public float localTimeScale = 1f;
	public float blendTime = 1f;

	public bool isPaused {
		get {
			return _isPaused;
		}
	}
	private bool _isPaused;

	// Local Time
	private float localTime = 0;

	// Refernces
	private Graphic _graphic;
	private IEnumerator currentColorBlend;
	private string warningMessage; 

	void Awake () {
		_graphic = GetComponent<Graphic> ();
		_isPaused = false;
		warningMessage = "Script \"ColorBlenderUIGraphic\" could NOT find a \"Graphic\" type component in GameObject \"" + gameObject.name + "\". Script was removed from " + gameObject.name;

		if (_graphic == null) {
			Debug.LogWarning (warningMessage);
			Destroy (this);
			return;
		}

	}


	void Start () {
		localTime = 0;
	}


	void Update () {
		if (!_isPaused) {
			localTime += Time.deltaTime * localTimeScale;
		}
	}


	public void UseAlternateColor () {
		_graphic.color = alternateColor;
	}


	public void UseStartColor () {
		_graphic.color = startColor;
	}


	public void UseEndColor () {
		_graphic.color = endColor;
	}


	public void Pause () {
		_isPaused = true;
	}


	public void UnPause () {
		_isPaused = false;
	}


	public IEnumerator StartColorBlend (bool hideOnEnd = false) {
		StopColorBlend ();
		gameObject.SetActive (true);
		currentColorBlend = ColorBlend ();
		yield return (StartCoroutine (currentColorBlend));
		currentColorBlend = null;
		gameObject.SetActive (!hideOnEnd);
	}


	public void StopColorBlend (bool hideOnEnd = false) {
		if (currentColorBlend != null) {
			StopCoroutine (currentColorBlend);
			currentColorBlend = null;
		}
		gameObject.SetActive (!hideOnEnd);
	}


	private IEnumerator ColorBlend () {

		float timeStart = localTime;
		_graphic.color = startColor;

		while (localTime - timeStart < blendTime) {

			while (_isPaused)
				yield return null;

			float u = (localTime - timeStart) / blendTime;
			_graphic.color = Color.Lerp (startColor, endColor, u);
			yield return null;
		}
		_graphic.color = endColor;

	}

}
