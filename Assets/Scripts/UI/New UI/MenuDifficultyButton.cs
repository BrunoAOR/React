using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDifficultyButton : MonoBehaviour {

	public MenuGameModePanel parentModePanel;
	public Difficulty difficulty;
	[HideInInspector] public Color unlockedColor;
	[HideInInspector] public Color lockedColor;
	public Image lockImage;
	[HideInInspector] public Color lockImageColor;
	public GameObject buttonText;
	public Text highscoreText;
	private bool _isUnlocked = true;
	private Image _image;

	void Awake () {
		_image = GetComponent<Image> ();
		lockImage.color = lockImageColor;
	}

	void Reset () {
		parentModePanel = GetComponentInParent<MenuGameModePanel> ();
	}

	public void SetButtonsColors (Color unlockedColor, Color lockedColor, Color lockImageColor) {
		this.unlockedColor = unlockedColor;
		this.lockedColor = lockedColor;
		this.lockImageColor = lockImageColor;
		if (_image == null) {
			_image = GetComponent<Image> ();
		}

		if (_isUnlocked) {
			_image.color = unlockedColor;
		} else {
			_image.color = lockedColor;
		}
		lockImage.color = lockImageColor;
	}

	public void UpdateUnlockStates (bool unlockState, UnlockCondition unlockCondition) {
		if (unlockState == true) {
			// Unlock
			_isUnlocked = true;
			_image.color = unlockedColor;
			lockImage.gameObject.SetActive (false);
			buttonText.SetActive (true);
			highscoreText.text = string.Format ("Highscore: {0}", Managers.Score.GetHighscore (parentModePanel.gameMode, difficulty).ToString ());
		} else {
			// Lock
			_isUnlocked = false;
			_image.color = lockedColor;
			lockImage.gameObject.SetActive (true);
			buttonText.SetActive (false);
			highscoreText.text = unlockCondition.GetText ();
		}
	}

	public void OnClick () {
		if (_isUnlocked) {
			parentModePanel.OnDifficultyButtonClicked (difficulty);
		} else {
			Managers.Audio.PlaySFX (SFX.IconClicked_Locked);
		}
	}
}
