using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDifficultyButton : MonoBehaviour {

	public MenuGameModePanel parentModePanel;
	public Difficulty difficulty;
	[HideInInspector] public Color unlockedColor;
	[HideInInspector] public Color lockedColor;
	public Text difficultyText;
	public Text highscoreText;
	public Image lockImage;
	[HideInInspector] public Color lockImageColor;
	private bool _isAnimating = false;
	private Image _image;
	private UIShaker _lockImageShaker;

	private bool _isUnlocked;
	private MenuLockState nextLockState = MenuLockState.Undefined;

	[Header ("Unlock Animation")]
	public float lockShrinkDuration = 2f;
	public float textAppearDuration = 1f;

	void Reset () {
		parentModePanel = GetComponentInParent<MenuGameModePanel> ();
		name = difficulty.ToString ();
	}

	void OnValidate () {
		name = difficulty.ToString ();
		if (difficultyText != null) {
			difficultyText.text = difficulty.ToString ();
		}
	}

	void Awake () {
		if (parentModePanel == null) {
			parentModePanel = GetComponentInParent<MenuGameModePanel> ();
		}
		_image = GetComponent<Image> ();
		_lockImageShaker = lockImage.GetComponent<UIShaker> ();
		difficultyText.text = difficulty.ToString ();
	}

	void Start () {
		lockImage.color = lockImageColor;
	}

	public void SetButtonsColors (Color unlockedColor, Color lockedColor, Color lockImageColor) {
		this.unlockedColor = unlockedColor;
		this.lockedColor = lockedColor;
		this.lockImageColor = lockImageColor;
	}

	public void UpdateUnlockStates (bool unlockState, UnlockCondition unlockCondition) {
		// If _isUnlocked is true, then no further action needs to be taken other than to update the highscoreText.
		if (_isUnlocked) {
			highscoreText.text = string.Format ("Highscore: {0}", Managers.Score.GetHighscore (parentModePanel.gameMode, difficulty).ToString ());
			return;
		}

		if (nextLockState == MenuLockState.Undefined) {
			// So, first time loading the menu...

			// Instantly set the right Locked/Unlocked stated (_isUnlocked will be set within these methods)
			if (unlockState == true) {
				Unlock ();
			} else {
				Lock (unlockCondition);
			}

			// Record the unlockState in nextLockState. Only if the current unlockState (and therefore nextLockState) is locked, will there be a chance for a future unlock animation.
			nextLockState = (MenuLockState)System.Convert.ToInt32 (unlockState);
		} else if (_isUnlocked == false && unlockState == true) {
			// So, any other time that the menu is loaded and a menu item will be unlocked but WAS locked
			nextLockState = MenuLockState.Unlocked;	// Changing nextLockState to Unlocked but leaving _isUnlocked as false allows for actions to be taken in the TriggerUnlockAnimations method.
		}

		// If _isUnlocked == true && unlockState == true, then no action needs to be taken, because nextLockState is already MenuLockState.Unlocked from a previous run.
		// If nextLockState == MenuLockState.Locked, then nothing needs to be done, because a menu item can't be locked on the first loading of the menu and therefore, no animation is required.

	}

	public void SkipUnlockAnimation () {
		Debug.Log ("Entered skip");
		if (_isUnlocked == false && nextLockState == MenuLockState.Unlocked) {
			Debug.Log ("Entered skip if");
			// So, only when there was meant to be an unlock animation
			Unlock ();
		}
	}

	private void Unlock () {
		_isUnlocked = true;
		_image.color = unlockedColor;
		lockImage.gameObject.SetActive (false);
		difficultyText.gameObject.SetActive (true);
		highscoreText.text = string.Format ("Highscore: {0}", Managers.Score.GetHighscore (parentModePanel.gameMode, difficulty).ToString ());
	}

	private void Lock (UnlockCondition unlockCondition) {
		_isUnlocked = false;
		_image.color = lockedColor;
		lockImage.gameObject.SetActive (true);
		difficultyText.gameObject.SetActive (false);
		highscoreText.text = unlockCondition.GetText ();
	}

	public IEnumerator TriggerUnlockAnimations () {
		// This method will only run if _isUnlocked == false and nextLockState is Unlockeed. Meaning that the menu item was unlocked in this menu loading.
		if (_isUnlocked == false && nextLockState == MenuLockState.Unlocked) {
			yield return (UnlockCoroutine ());
		}
	}

	private IEnumerator UnlockCoroutine () {
		_isAnimating = true;

		// Shake the lock
		_lockImageShaker.StartShakeRotate ();

		while (_lockImageShaker.IsShaking ()) {
			yield return null;
		}

		float timeStart = Time.time;
		float u;
		Vector3 startScale = lockImage.transform.localScale;
		Vector3 targetScale = Vector3.zero;
		Color startColor = highscoreText.color;
		Color dimmedColor = startColor;
		dimmedColor.a = 0;

		// Shrink lockImage and dissapear the unlock condition
		while ((Time.time - timeStart) < lockShrinkDuration) {
			u = (Time.time - timeStart) / lockShrinkDuration;
			lockImage.transform.localScale = Vector3.Lerp (startScale, targetScale, u);
			highscoreText.color = Color.Lerp (startColor, dimmedColor, u);
			yield return null;
		}

		// Reset lockImage scale and turn it off
		lockImage.transform.localScale = startScale;
		lockImage.gameObject.SetActive (false);

		// Change the button color
		_image.color = unlockedColor;

		// Replace the highscore text with the actual highscore Text (no longer the unlck condition
		highscoreText.text = string.Format ("Highscore: {0}", Managers.Score.GetHighscore (parentModePanel.gameMode, difficulty).ToString ());

		// Appear both the difficultyText and the highscore Text
		difficultyText.gameObject.SetActive (true);
		difficultyText.color = dimmedColor;

		timeStart = Time.time;

		while ((Time.time - timeStart) < textAppearDuration) {
			u = (Time.time - timeStart) / textAppearDuration;
			difficultyText.color = Color.Lerp (dimmedColor, startColor, u);
			highscoreText.color = Color.Lerp (dimmedColor, startColor, u);
			yield return null;
		}

		difficultyText.color = startColor;
		highscoreText.color = startColor;

		// Set _isUnlocked to true to allow clicking after the animation.
		_isUnlocked = true;

		_isAnimating = false;
	}

	public void OnClick () {
		if (_isUnlocked) {
			parentModePanel.OnDifficultyButtonClicked (difficulty);
		} else if (!_isAnimating){
			// If the button is animating (unlocking), nothing happens when it is clicked
			Managers.Audio.PlaySFX (SFX.IconClicked_Locked);
			_lockImageShaker.StartShakeRotate ();
		}
	}
}
