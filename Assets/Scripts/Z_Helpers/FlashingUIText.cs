using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Text))]
public class FlashingUIText : MonoBehaviour {

	public Color dimColor = new Color (1f, 1f, 1f, 0.25f);
	public Color highlightColor = new Color (1f, 1f, 1f, 1f);
	public float dimScale = 0.9f;
	public float highlightScale = 1.0f;
	public float cycleDuration = 0.5f;

	private Text _text;
	private Color startColor;
	private Vector3 startScale;
	private float startTime;
	private float cycleTime;
	private float u;
	private Color currentColor;
	private Vector3 currentScale;
	private float halfCycle {
		get {
			return (cycleDuration / 2);
		}
	}

	void Awake () {
		_text = GetComponent<Text> ();
		startColor = _text.color;
		startScale = _text.gameObject.transform.localScale;
	}


	void OnEnable () {
		startTime = Time.time;
	}


	void OnDisable () {
		_text.color = startColor;
		_text.gameObject.transform.localScale = startScale;
	}

	void Update () {
		cycleTime = (Time.time - startTime) % cycleDuration;

		if (cycleTime <= halfCycle) {
			// HIGHLIGHTING
			u = cycleTime / halfCycle;
			currentColor = Color.Lerp (dimColor, highlightColor, u);
			currentScale = Vector3.one * ((1 - u) * dimScale + u * highlightScale);
		} else {
			// DIMMING
			u = (cycleTime - halfCycle) / halfCycle;
			currentColor = Color.Lerp (highlightColor, dimColor, u);
			currentScale = Vector3.one * ((1 - u) * highlightScale + u * dimScale);
		}

		_text.color = currentColor;
		_text.gameObject.transform.localScale = currentScale;

	}
}
