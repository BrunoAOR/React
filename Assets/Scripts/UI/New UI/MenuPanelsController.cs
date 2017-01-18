﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelsController : MonoBehaviour {

	public enum MenuDirection {
		Out = -1,
		In = 1
	}

	public MenuController menuController;
	public MenuGameModePanel[] gameModePanels;
	private int currentPanelIndex;
	public float sideScrollDuration = 0.5f;
	[Range (0f, 1f)]
	public float sideScrollAfterBounce = 0.35f;
	public float inOutScrollDuration = 0.5f;
	[Range (0f, 1f)]
	public float inOutScrollBounce = 0.2f;
	public float scrollOutYOffset = 1136f;

	private float _originalYPos;
	private bool _isAnimating;

	void Awake () {
		if (menuController == null) {
			menuController = GetComponentInParent<MenuController> ();
		}
		if (gameModePanels == null) {
			GetComponentsInChildren<MenuGameModePanel> ();
		}

		_isAnimating = false;
		_originalYPos = transform.localPosition.y;
		currentPanelIndex = 0;
		gameModePanels [currentPanelIndex].SetEnabled (true);

		for (int i = 1; i < gameModePanels.Length; i++) {
			gameModePanels [i].SetEnabled (false);
		}

		gameModePanels [0].ArrowVisible (MenuArrow.Direction.Left, false);
		gameModePanels [gameModePanels.Length - 1].ArrowVisible (MenuArrow.Direction.Right, false);

		// Start with the panels below (negative y) the screen
		transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y - scrollOutYOffset, transform.localPosition.z);
	}

	void Reset () {
		menuController = GetComponentInParent<MenuController> ();
		gameModePanels = GetComponentsInChildren<MenuGameModePanel> ();
	}

	public bool IsAnimating () {
		return (_isAnimating);
	}

	public void SetButtonsColors (Color unlockedColor, Color lockedColor, Color lockImageColor) {
		for (int i = 0; i < gameModePanels.Length; i++) {
			gameModePanels[i].SetButtonsColors (unlockedColor, lockedColor, lockImageColor);
		}
	}

	public void ArrowVisible (int panelIndex, MenuArrow.Direction direction, bool isVisible) {
		if (panelIndex < 0 || panelIndex >= gameModePanels.Length) {
			Debug.LogError ("panelIndex provided is out of bounds!");
			return;
		}

		gameModePanels [panelIndex].ArrowVisible (direction, isVisible);
	}

	public void UpdateUnlockStates (bool[][] unlockStates, UnlockCondition[][] unlockConditions) {
		if (gameModePanels.Length != unlockStates.Length) {
			Debug.LogError ("The number of panel unlock states passed in (" + unlockStates.Length + 
				") is different from the number of game mode panels (" + gameModePanels.Length + 
				") available!"
			);
			return;
		}

		for (int i = 0; i < gameModePanels.Length; i++) {
			gameModePanels [i].UpdateUnlockStates (unlockStates [i], unlockConditions[i]);
		}
	}

	public bool PanelCanShowRightArrow (MenuGameModePanel modePanel) {
		int panelIndex = -1;

		for (int i = 0; i < gameModePanels.Length; i++) {
			if (modePanel == gameModePanels [i]) {
				panelIndex = i;
				break;
			}
		}

		if (panelIndex == -1) {
			//So, if the modePanel does not belong to gameModePanels
			Debug.LogError (modePanel.name + " attempted to call PanelCanShowRightArrow() on " + this.name + ", but " + modePanel.name + " is not part of the gameModePanels array!");
		}

		if (panelIndex == gameModePanels.Length - 1) {
			// So, if the modePanel is the last panel
			return false;
		}

		return true;
	}

	public void OnArrowClicked (MenuGameModePanel callingPanel, MenuArrow.Direction direction) {
		if (direction == MenuArrow.Direction.Left && currentPanelIndex == 0)
			return;

		if (direction == MenuArrow.Direction.Right && currentPanelIndex == gameModePanels.Length - 1)
			return;

		Debug.Log ("Scrolling " + direction + " from " + gameModePanels [currentPanelIndex].gameMode.ToString () + " to " + gameModePanels [currentPanelIndex + (int)direction].gameMode.ToString ());

		StartCoroutine (ScrollPanelsTowards (direction));
	}

	public void OnDifficultyButtonClicked (GameMode gameMode, Difficulty difficulty) {
		menuController.LaunchGame (gameMode, difficulty);
	}

	private IEnumerator ScrollPanelsTowards (MenuArrow.Direction direction) {
		_isAnimating = true;
		gameModePanels [currentPanelIndex].SetEnabled (false);

		float startXPos = transform.localPosition.x;
		float targetXPos = -gameModePanels [currentPanelIndex + (int)direction].transform.localPosition.x;

		Vector3 currentPos = transform.localPosition;
		float u = 0;
		float n = 0;
		float u2 = 0;
		float startTime = Time.time;

		while ((Time.time - startTime) < sideScrollDuration) {
			u = (Time.time - startTime) / sideScrollDuration;

			if (u <= 0.5) {
				n = 1 + 2 * Mathf.PI * sideScrollAfterBounce;
				u2 = Mathf.Pow (2, n - 1) * Mathf.Pow (u, n);
			} else {
				u2 = u - sideScrollAfterBounce * Mathf.Sin (2 * Mathf.PI * u);
			}

			currentPos.x = (1 - u2) * startXPos + u2 * targetXPos;
			transform.localPosition = currentPos;
			yield return null;
		}

		currentPos.x = targetXPos;
		transform.localPosition = currentPos;

		currentPanelIndex += (int)direction;
		gameModePanels [currentPanelIndex].SetEnabled (true);
		_isAnimating = false;
	}

	public IEnumerator ScrollInOut (MenuDirection direction) {
		_isAnimating = true;
		if (direction == MenuDirection.Out) {
			gameModePanels [currentPanelIndex].SetEnabled (false);
		}

		float startYPos = transform.localPosition.y;
		float targetYPos = 0;
		if (direction == MenuDirection.In) {
			targetYPos = _originalYPos;
		} else {
			targetYPos = _originalYPos - scrollOutYOffset;
		}

		Vector3 currentPos = transform.localPosition;
		float u = 0;
		float n = 0;
		float u2 = 0;
		float startTime = Time.time;

		while ((Time.time - startTime) < inOutScrollDuration) {
			u = (Time.time - startTime) / inOutScrollDuration;

			if (u <= 0.5) {
				n = 1 + 2 * Mathf.PI * inOutScrollBounce;
				u2 = Mathf.Pow (2, n - 1) * Mathf.Pow (u, n);
			} else {
				u2 = u - inOutScrollBounce * Mathf.Sin (2 * Mathf.PI * u);
			}

			currentPos.y = (1 - u2) * startYPos + u2 * targetYPos;
			transform.localPosition = currentPos;
			yield return null;
		}

		currentPos.y = targetYPos;
		transform.localPosition = currentPos;

		if (direction == MenuDirection.In) {
			gameModePanels [currentPanelIndex].SetEnabled (true);
		}
		_isAnimating = false;
	}

}
