using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSText : MonoBehaviour {
	public int n = 100;

	private Text _text;
	private float[] fpsSamples;
	private int sIndex = 0;

	void Awake () {
		_text = GetComponent<Text> ();
		fpsSamples = new float[n];
	}
	
	// Update is called once per frame
	void Update () {
		fpsSamples [sIndex] = (1f / Time.deltaTime);
		sIndex++;
		if (sIndex >= fpsSamples.Length)
			sIndex = 0;

		float fpsAverage = 0;
		for (int i = 0; i < fpsSamples.Length; i++) {
			fpsAverage += fpsSamples [i];
		}
		fpsAverage /= fpsSamples.Length;

		_text.text = fpsAverage.ToString ("F1") + " FPS";
	}
}
