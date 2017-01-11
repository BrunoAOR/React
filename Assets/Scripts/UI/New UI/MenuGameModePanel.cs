using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGameModePanel : MonoBehaviour {

	public MenuPanelsController parentPanelsController;
	public GameMode gameMode;
	public CanvasGroup canvasGroup;
	[Range (0f, 1f)]
	public float alphaWhenDisabled = 0.5f;
	private bool interactable;

	void Awake () {
		SetEnabled (false);
	}

	void Reset () {
		parentPanelsController = GetComponentInParent<MenuPanelsController> ();
		canvasGroup = GetComponent<CanvasGroup> ();
	}

	public void OnArrowClicked (MenuArrow.Direction direction) {
		if (!interactable)
			return;

		Debug.Log (direction.ToString () + " arrow clicked on " + gameMode.ToString () + " mode.");
		parentPanelsController.OnArrowClicked (this, direction);
	}

	public void OnDifficultyButtonClicked (Difficulty difficulty) {
		if (!interactable)
			return;

		Debug.Log (difficulty.ToString () + " button clicked on " + gameMode.ToString () + " mode.");
	}

	public void SetEnabled (bool _) {
		if (_) {
			// Enable
			interactable = true;
			canvasGroup.alpha = 1f;
		} else {
			// Disable
			interactable = false;
			canvasGroup.alpha = alphaWhenDisabled;
		}
	}
}
