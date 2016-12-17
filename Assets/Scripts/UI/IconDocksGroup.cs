using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IconDocksGroup : MonoBehaviour {
	public MoveRotateUI [] iconDocks;

	[Header ("Unlocked Icons")]
	public Color normalColor = Color.white;
	public float initialScale = 1.0f;
	public float selectionDuration = 0.2f;
	public float selectionScaleUp = 1.2f;
	public Color selectedColor = new Color (0.5f, 0.5f, 1f, 1f);
	public float deselectionDuration = 0.1f;

	[Header ("Locked Icons")]
	public Vector3 initialRotation = Vector3.zero;
	public float shakeAngleAmplitude = 15f;
	public float shakeCycleDuration = 0.25f;
	public int shakeCycles = 4;

	private Image[] iconDocksImages;
	private IEnumerator[] iconImagesCoroutines;
	private int selectedIconIndex = -1;

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
		iconImagesCoroutines = new IEnumerator[iconDocks.Length];
	}


	public void OnIconDockClicked (Image image, bool iconIsUnlocked) {
		// Find the imageIndex that correspond to the provided image
		int imageIndex = -1;
		for (int i = 0; i < iconDocksImages.Length; i++) {
			if (iconDocksImages [i] == image) {
				imageIndex = i;
				break;
			}
		}

		// If the imageIndex remains at -1, then the image does not belong to this group
		if (imageIndex == -1) {
			Debug.LogWarning ("IconDocksGroup named " + gameObject.name + " received a call from an image (" + image.name + ") that doesn belong to its group");
			return;
		}

		// So we have a valid image that belongs to this IconDocksGroup
		// Now we check if that image (Icon) isUnlocked
		if (!iconIsUnlocked) {
			// So the icon is locked. We'll shake the iconImage.transform by rotating it, thereby shaking the child iconLock image
			if (iconImagesCoroutines [imageIndex] != null) {
				StopCoroutine (iconImagesCoroutines [imageIndex]);
				ResetImageRotation (imageIndex);
			}
			iconImagesCoroutines [imageIndex] = ShakeRotateImage (imageIndex);
			StartCoroutine (iconImagesCoroutines [imageIndex]);
			// And then we stop execution, because we don't need the icons selection/deselection code for a locked icon
			return;
		}

		// Now we know the icon is unlocked so we proceed to the icons selection/deselection code

		// If the imageIndex is equal to the selectedIconIndex, then we shouldn't do anything
		if (imageIndex == selectedIconIndex)
			return;

		// Deselect the previous selectedIconImage which has selectedIconIndex
		if (selectedIconIndex != -1) {
			if (iconImagesCoroutines [selectedIconIndex] != null) {
				StopCoroutine (iconImagesCoroutines [selectedIconIndex]);
				ResetImageScale (selectedIconIndex);
			}
			iconImagesCoroutines [selectedIconIndex] = DeselectIconImage (selectedIconIndex);
			StartCoroutine (iconImagesCoroutines [selectedIconIndex]); 
		}

		// Change the selectedIconIndex to the current one
		selectedIconIndex = imageIndex;
		if (iconImagesCoroutines [selectedIconIndex] != null) {
			StopCoroutine (iconImagesCoroutines [selectedIconIndex]);
			ResetImageScale (selectedIconIndex);
		}
		iconImagesCoroutines [selectedIconIndex] = SelectIconImage (selectedIconIndex);
		StartCoroutine (iconImagesCoroutines [selectedIconIndex]);
	}


	private void ResetImageScale (int imageIndex) {
		iconDocksImages [imageIndex].transform.localScale = initialScale * Vector3.one;
	}


	private IEnumerator SelectIconImage (int imageIndex) {
		Image image = iconDocksImages [imageIndex];
		Vector3 initialScaleV = initialScale * Vector3.one;
		Vector3 selectionsScaleV = selectionScaleUp * Vector3.one;

		float timeStart = Time.time;
		float u;

		while ( (Time.time - timeStart) < selectionDuration) {
			u = (Time.time - timeStart) / selectionDuration;
			image.color = Color.Lerp (normalColor, selectedColor, u);
			image.transform.localScale = Vector3.Lerp (selectionsScaleV, initialScaleV, u);
			yield return null;
		}

		image.color = selectedColor;
		ResetImageScale (imageIndex);

		iconImagesCoroutines [imageIndex] = null;
	}


	private IEnumerator DeselectIconImage (int imageIndex) {
		Image image = iconDocksImages [imageIndex];
		float timeStart = Time.time;
		float u;

		while ( (Time.time - timeStart) < deselectionDuration) {
			u = (Time.time - timeStart) / deselectionDuration;
			image.color = Color.Lerp (selectedColor, normalColor, u);
			yield return null;
		}

		image.color = normalColor;

		iconImagesCoroutines [imageIndex] = null;
	}


	private void ResetImageRotation (int imageIndex) {
		SetImageRotation (imageIndex, Quaternion.Euler(initialRotation));
	}


	private void SetImageRotation (int imageIndex, Vector3 rotation) {
		SetImageRotation (imageIndex, Quaternion.Euler (rotation));
	}

	private void SetImageRotation (int imageIndex, Quaternion rotation) {
		iconDocksImages [imageIndex].transform.localRotation = rotation;
	}

	private IEnumerator ShakeRotateImage (int imageIndex) {
		// Clockwise = -z
		float subCycleDuration = shakeCycleDuration / 4f;
		float zeroZRotation = initialRotation.z;
		float maxZRotation = initialRotation.z + shakeAngleAmplitude;
		float minZRotation = initialRotation.z - shakeAngleAmplitude;
		float cycles = 0;
		Vector3 currentRotation = initialRotation;
		float cycleStart;
		float u;

		while (cycles < shakeCycles) {
			// First 1/4 cycle: Going from zeroZRotation to maxZRotation
			cycleStart = Time.time;
			while ( (Time.time - cycleStart) < subCycleDuration) {
				u = (Time.time - cycleStart) / subCycleDuration;
				currentRotation.z = (1 - u) * zeroZRotation + u * maxZRotation;
				SetImageRotation (imageIndex, currentRotation);
				yield return null;
			}

			// Next 1/2 cycle: Going from maxZRotation to minZRotation
			cycleStart = Time.time;
			while ( (Time.time - cycleStart) < (2 * subCycleDuration)) {
				u = (Time.time - cycleStart) / (2 * subCycleDuration);
				currentRotation.z = (1 - u) * maxZRotation + u * minZRotation;
				SetImageRotation (imageIndex, currentRotation);
				yield return null;
			}

			// Last 1/4 cycle: Going from minZRotation to zeroZRotation
			cycleStart = Time.time;
			while ( (Time.time - cycleStart) < subCycleDuration) {
				u = (Time.time - cycleStart) / subCycleDuration;
				currentRotation.z = (1 - u) * minZRotation + u * zeroZRotation;
				SetImageRotation (imageIndex, currentRotation);
				yield return null;
			}

			cycles++;
		}

		ResetImageRotation (imageIndex);
		iconImagesCoroutines [imageIndex] = null;
	}

}
