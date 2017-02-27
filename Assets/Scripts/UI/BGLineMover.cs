using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BGLineMover : MonoBehaviour, IPoolable<BGLineMover> {

	public float speed = 1;

	public RectTransform rectTransform;
	public Image image;

	public UnspawnDelegate<BGLineMover> Unspawn { get ; set; }

	private float screenWidth;


	void Reset () {
		rectTransform = GetComponent<RectTransform> ();
		image = GetComponent <Image> ();
	}

	void Start () {
		screenWidth = Camera.main.pixelWidth;
	}
	
	void Update () {
		transform.Translate (Vector3.right * screenWidth * speed * Time.deltaTime, Space.Self);
	}

	public void ReturnToPoolAfter (float time) {
		Invoke ("ReturnToPool", time);
	}

	private void ReturnToPool () {
		if (Unspawn != null) {
			Unspawn (this);
		} else {
			Destroy (gameObject);
		}
	}

}
