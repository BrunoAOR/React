using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuLockState {
	Undefined = -1,
	Locked = 0,		// So, unlockState = false (-> 0);
	Unlocked = 1,	// So, unlockState = true  (-> 1);
}

public class MenuGameModePanel : MonoBehaviour {

	public GameMode gameMode;
	public MenuPanelsController parentPanelsController;
	public MenuDifficultyButton[] difficultyButtons;
	public MenuArrow[] menuArrows;
	public Text gameModeName;
	public Text descriptionText;
	public GameObject difficultiesPanel;
	public GameObject generalLockImage;
	public CanvasGroup canvasGroup;
	[Range (0f, 1f)]
	public float alphaWhenDisabled = 0.5f;
	private bool _interactable;
	private UIShaker _generalLockShaker;
	private CanvasGroup _difficultiesPanelCanvasGroup;

	[Header ("Unlock Animation")]
	public float lockShrinkDuration = 2f;
	public float textAppearDuration = 1f;

	private bool _isUnlocked;
	private MenuLockState _nextLockState = MenuLockState.Undefined;
	private bool _isAnimating = false;

	void Reset () {
		parentPanelsController = GetComponentInParent<MenuPanelsController> ();
		difficultyButtons = GetComponentsInChildren<MenuDifficultyButton> ();
		menuArrows = GetComponentsInChildren<MenuArrow> ();
		Transform tT;
		tT = transform.Find ("GameModeName");
		if (tT != null) {
			gameModeName = tT.GetComponent<Text> ();
		}
		tT = transform.Find ("BottomSection/GameModeDescription");
		if (tT != null) {
			descriptionText = tT.GetComponent<Text> ();
		}
		tT = transform.FindChild ("BottomSection/Difficulties");
		if (tT != null) {
			difficultiesPanel = tT.gameObject;
		}
		MenuGeneralLock tMGL = GetComponentInChildren<MenuGeneralLock> (true);
		if (tMGL != null) {
			generalLockImage = tMGL.gameObject;
		}
		canvasGroup = GetComponent<CanvasGroup> ();
		OnValidate ();
	}

	void OnValidate () {
		name = gameMode.ToString ();
		if (gameModeName != null) {
			gameModeName.text = name + " Mode";
		}
	}

	void Awake () {
		if (parentPanelsController == null) {
			parentPanelsController = GetComponentInParent<MenuPanelsController> ();
		}
		_generalLockShaker = generalLockImage.GetComponent<UIShaker> ();
		_difficultiesPanelCanvasGroup = difficultiesPanel.GetComponent<CanvasGroup> ();
	}

	public bool IsAnimating () {
		return (_isAnimating);
	}

	public bool IsUnlocked () {
		return (_isUnlocked);
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

		if (_nextLockState == MenuLockState.Undefined) {
			// So, first time loading the menu...

			// Instantly set the right Locked/Unlocked stated (_isUnlocked will be set within these methods)
			if (anyUnlocked) {
				Unlock ();
			} else {
				Lock (unlockConditions [0]);
			}

			// Record the unlockState in nextLockState. Only if the current unlockState (and therefore nextLockState) is locked, will there be a chance for a future unlock animation.
			_nextLockState = (MenuLockState)System.Convert.ToInt32 (anyUnlocked);
		} else if (_isUnlocked == false && anyUnlocked == true) {
			// So, any other time that the menu is loaded and a menu item will be unlocked but WAS locked
			_nextLockState = MenuLockState.Unlocked;	// Changing nextLockState to Unlocked but leaving _isUnlocked as false allows for actions to be taken in the TriggerUnlockAnimations method.
		}

		// If _isUnlocked == true && anyUnlocked == true, then no action needs to be taken, because nextLockState is already MenuLockState.Unlocked from a previous run.
		// If nextLockState == MenuLockState.Locked, then nothing needs to be done, because a menu item can't be locked on the first loading of the menu and therefore, no animation is required.

	}

	private void Unlock () {
		_isUnlocked = true;
		gameModeName.text = string.Format("{0} Mode", gameMode.ToString ());
		difficultiesPanel.SetActive (true);
		generalLockImage.SetActive (false);
		if (parentPanelsController.PanelCanShowRightArrow (this)) {
			ArrowVisible (MenuArrow.Direction.Right, true);
		}
		descriptionText.text = Managers.Enums.GetGameModeLogic (gameMode).modeDescription;
	}

	private void Lock (UnlockCondition unlockCondition) {
		_isUnlocked = false;
		gameModeName.text = "MODE LOCKED!";
		difficultiesPanel.SetActive (false);
		generalLockImage.SetActive (true);
		ArrowVisible (MenuArrow.Direction.Right, false);
		descriptionText.text = unlockCondition.GetText ();
	}

	public void DisplayFullStats (bool fullDisplay) {
		for (int i = 0; i < difficultyButtons.Length; i++) {
			difficultyButtons [i].DisplayFullStats (fullDisplay);
		}
	}

	private IEnumerator TriggerUnlockAnimations () {
		_isAnimating = true;
		if (_isUnlocked == false && _nextLockState == MenuLockState.Unlocked) {
			// Since the first difficulty to become available is Easy (first button), its animation will also attempt to run on this panel loading.
			// Therefore, its animation must be skipped.
			difficultyButtons[0].SkipUnlockAnimation ();
			yield return (UnlockCoroutine ());
			parentPanelsController.DisplayFullStatsButton (true);
		} else if (_isUnlocked) {
			// Animations in the children menu items can only happen if the menu is unlocked.
			for (int i = 0; i < difficultyButtons.Length; i++) {
				yield return (difficultyButtons [i].TriggerUnlockAnimations ());
			}
		}
		_isAnimating = false;
	}

	private IEnumerator UnlockCoroutine () {
		// Shake the lock
		_generalLockShaker.StartShakeRotate ();

		while (_generalLockShaker.IsShaking ()) {
			yield return null;
		}

		// Change gameModeName text to Unlocking...
		gameModeName.text = "Unlocking...";

		float timeStart = Time.time;
		float u;
		Vector3 startScale = generalLockImage.transform.localScale;
		Vector3 targetScale = Vector3.zero;
		Color startColor = descriptionText.color;
		Color dimmedColor = startColor;
		dimmedColor.a = 0;

		while ((Time.time - timeStart) < lockShrinkDuration) {
			u = (Time.time - timeStart) / lockShrinkDuration;
			generalLockImage.transform.localScale = Vector3.Lerp (startScale, targetScale, u);
			descriptionText.color = Color.Lerp (startColor, dimmedColor, u);
			yield return null;
		}

		descriptionText.color = dimmedColor;

		// Reset lockImage scale and turn it off
		generalLockImage.transform.localScale = startScale;
		generalLockImage.gameObject.SetActive (false);

		// Change gameModeName text to the actual game mode name
		gameModeName.text = string.Format ("{0} Mode", gameMode.ToString ());

		// Change the description text to the actual game mode description
		descriptionText.text = Managers.Enums.GetGameModeLogic(gameMode).modeDescription;

		// Turn on the difficulties panel
		difficultiesPanel.gameObject.SetActive (true);

		timeStart = Time.time;

		while ((Time.time - timeStart) < textAppearDuration) {
			u = (Time.time - timeStart) / textAppearDuration;
			descriptionText.color = Color.Lerp (dimmedColor, startColor, u);
			_difficultiesPanelCanvasGroup.alpha = u;
			yield return null;
		}

		descriptionText.color = startColor;
		_difficultiesPanelCanvasGroup.alpha = 1f;

		// Make the right arrow visible
		if (parentPanelsController.PanelCanShowRightArrow (this)) {
			ArrowVisible (MenuArrow.Direction.Right, true);
		}

		// Set _isUnlocked to true.
		_isUnlocked = true;
	}

	public void OnArrowClicked (MenuArrow.Direction direction) {
		if (!_interactable || _isAnimating)
			return;

		Debug.Log (direction.ToString () + " arrow clicked on " + gameMode.ToString () + " mode.");
		Managers.Audio.PlaySFX (SFX.MenuButton);
		parentPanelsController.OnArrowClicked (this, direction);
	}

	public void OnDifficultyButtonClicked (Difficulty difficulty) {
		if (!_interactable || _isAnimating)
			return;

		Debug.Log (difficulty.ToString () + " button clicked on " + gameMode.ToString () + " mode.");
		parentPanelsController.OnDifficultyButtonClicked (gameMode, difficulty);
	}

	public void SetEnabled (bool _) {
		if (_) {
			// Enable
			_interactable = true;
			canvasGroup.alpha = 1f;
			StartCoroutine (TriggerUnlockAnimations ());
		} else {
			// Disable
			_interactable = false;
			canvasGroup.alpha = alphaWhenDisabled;
		}
	}
}
