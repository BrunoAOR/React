using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuGameModePanel : MonoBehaviour {

	public MenuPanelsController parentPanelsController;
	public MenuDifficultyButton[] difficultyButtons;
	public MenuArrow[] menuArrows;
	public Text descriptionText;
	public GameObject difficultiesPanel;
	public GameObject generalLockImage;
	public GameMode gameMode;
	public CanvasGroup canvasGroup;
	[Range (0f, 1f)]
	public float alphaWhenDisabled = 0.5f;
	private bool interactable;

	void Reset () {
		parentPanelsController = GetComponentInParent<MenuPanelsController> ();
		difficultyButtons = GetComponentsInChildren<MenuDifficultyButton> ();
		menuArrows = GetComponentsInChildren<MenuArrow> ();
		canvasGroup = GetComponent<CanvasGroup> ();
	}

	public void SetButtonsColors (Color unlockedColor, Color lockedColor, Color lockImageColor) {
		for (int i = 0; i < difficultyButtons.Length; i++) {
			difficultyButtons [i].SetButtonsColors (unlockedColor, lockedColor, lockImageColor);
		}
	}

	public void ArrowVisible (MenuArrow.Direction direction, bool isVisible) {
		for (int i = 0; i < menuArrows.Length; i++) {
			if (menuArrows [i].direction == direction) {
				menuArrows [i].gameObject.SetActive (isVisible);
			}
		}
	}

	public void UpdateUnlockStates (bool [] difficultyUnlockStates, UnlockCondition[] unlockConditions) {
		if (difficultyButtons.Length != difficultyUnlockStates.Length) {
			Debug.LogError ("The number of unlock states passed in (" + difficultyUnlockStates.Length + 
				") is different from the number of difficulty buttons (" + difficultyButtons.Length + 
				") in the " + gameMode.ToString() + " game mode panel!"
			);
			return;
		}

		for (int i = 0; i < difficultyButtons.Length; i++) {
			difficultyButtons [i].UpdateUnlockStates (difficultyUnlockStates [i], unlockConditions[i]);
		}

		bool anyUnlocked = false;
		for (int i = 0; i < difficultyUnlockStates.Length; i++) {
			anyUnlocked |= difficultyUnlockStates [i];
		}
			
		if (anyUnlocked) {
			// Remove General Lock
			difficultiesPanel.SetActive (true);
			generalLockImage.SetActive (false);
			ArrowVisible (MenuArrow.Direction.Right, true);
			descriptionText.text = Managers.Enums.GetGameModeLogic (gameMode).modeDescription;
		} else {
			// Apply General Lock
			difficultiesPanel.SetActive (false);
			generalLockImage.SetActive (true);
			ArrowVisible (MenuArrow.Direction.Right, false);
			descriptionText.text = unlockConditions[0].GetText ();
		}
	}

	public void OnArrowClicked (MenuArrow.Direction direction) {
		if (!interactable)
			return;

		Debug.Log (direction.ToString () + " arrow clicked on " + gameMode.ToString () + " mode.");
		Managers.Audio.PlaySFX (SFX.MenuButton);
		parentPanelsController.OnArrowClicked (this, direction);
	}

	public void OnDifficultyButtonClicked (Difficulty difficulty) {
		if (!interactable)
			return;

		Debug.Log (difficulty.ToString () + " button clicked on " + gameMode.ToString () + " mode.");
		parentPanelsController.OnDifficultyButtonClicked (gameMode, difficulty);
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
