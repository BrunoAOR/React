using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IconDocksGroup : MonoBehaviour {
	public MoveRotateUI [] iconDocks;
	public Color normalColor = Color.white;
	public Color selectedColor = new Color (0.5f, 0.5f, 1f, 1f);

	private Image[] iconDocksImages;

	void Awake () {
		iconDocksImages = new Image[iconDocks.Length];
		for (int i = 0; i < iconDocksImages.Length; i++) {
			Image tImage = iconDocks [i].GetComponentInChildren<Image> ();
			if (tImage == null) {
				Debug.LogWarning ("IconDock " + i + "has no image in its children!");
			} else {
				iconDocksImages [i] = tImage;
			}
		}
	}


	public void OnIconDockClicked (Image image) {
		ResetImages ();
		image.color = selectedColor;
	}


	private void ResetImages () {
		for (int i = 0; i < iconDocksImages.Length; i++) {
			iconDocksImages [i].color = normalColor;
		}
	}
}
