using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour {

	[Header ("In/Out Animation")]
	public Vector2 inPosition;
	public Vector2 outPosition;
	public float scrollVerticalDuration = 0.5f;
	public float scrollHorizontalDuration = 0.5f;

	[Header ("Audio Toggles")]
	public Color enabledColor;
	public Color disabledColor;

	private bool _musicEnabled;
	private bool _soundEnabled;

	[Header ("Language selection")]
	public GameObject[] flagRims;

	private int _selectedLanguageIndex;

	[Header ("References")]
	public Image musicToggle;
	public Image soundToggle;

	[HideInInspector]
	public MenuController menuController;

	private RectTransform _rectTransform;
	private Vector3 _midPosition;

	private bool _interactable {
		get {
			Debug.Log ("controller says: " +menuController.CanShowSettingsPanel ());
			return (menuController.CanShowSettingsPanel ());
		}
	}
	private bool _isAnimating = false;
	private bool _isShowing = false;

	void Awake () {
		_rectTransform = GetComponent<RectTransform> ();
		_midPosition = new Vector3 (outPosition.x, inPosition.y, 0);
		_rectTransform.anchoredPosition = outPosition;

		if (Managers.Audio.musicMute) {
			_musicEnabled = false;
			musicToggle.color = disabledColor;
		} else {
			_musicEnabled = true;
			musicToggle.color = enabledColor;
		}

		if (Managers.Audio.soundMute) {
			_soundEnabled = false;
			soundToggle.color = disabledColor;
		} else {
			_soundEnabled = true;
			soundToggle.color = enabledColor;
		}

		// TODO: Get the actual saved selectedLanguage from the LanguageManager
		_selectedLanguageIndex = Managers.Language.GetActiveLanguageIndex();
		ToggleToFlag (_selectedLanguageIndex);
	}

	public void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	public void OnSettingsIconClicked () {
		if (!_interactable || _isAnimating) {
			return;
		}
		Managers.Audio.PlaySFX (SFX.IconClicked_NewSection);

		StartCoroutine (ShowHidePanel ());
	}

	public void OnMusicClicked () {
		if (!_interactable || _isAnimating) {
			return;
		}

		PlayMenuButtonSound ();
		if (_musicEnabled) {
			// Was on, Turn OFF
			Managers.Audio.musicMute = true;
			musicToggle.color = disabledColor;
			_musicEnabled = false;
		} else {
			// Was off, turn ON
			Managers.Audio.musicMute = false;
			musicToggle.color = enabledColor;
			_musicEnabled = true;
		}

	}

	public void OnSoundClicked () {
		if (!_interactable || _isAnimating) {
			return;
		}

		if (_soundEnabled) {
			// Was on, Turn OFF
			Managers.Audio.soundMute = true;
			soundToggle.color = disabledColor;
			_soundEnabled = false;
		} else {
			// Was off, turn ON
			Managers.Audio.soundMute = false;
			soundToggle.color = enabledColor;
			_soundEnabled = true;
			PlayMenuButtonSound ();
		}
	}

	public void OnFlagClicked (int index) {
		if (!_interactable || _isAnimating) {
			return;
		}

		PlayMenuButtonSound ();
		ToggleToFlag (index);
	}

	private void PlayMenuButtonSound () {
		Managers.Audio.PlaySFX (SFX.MenuButton);
	}

	private void ToggleToFlag (int flagIndex) {
		if (flagIndex < 0 || flagIndex >= flagRims.Length) {
			return;
		}

		for (int i = 0; i < flagRims.Length; i++) {
			flagRims [i].SetActive (false);
		}
		flagRims [flagIndex].SetActive (true);

		Managers.Language.SetActiveLanguage ((Languages)flagIndex);

		_selectedLanguageIndex = flagIndex;
	}

	private IEnumerator ShowHidePanel () {
		_isAnimating = true;

		if (!_isShowing) {
			// Panel should go in...
			menuController.WillShowSettingsPanel ();
			yield return (Scroll (_rectTransform, outPosition, _midPosition, scrollVerticalDuration));
			yield return (Scroll (_rectTransform, _midPosition, inPosition, scrollHorizontalDuration));
			_isShowing = true;
		} else {
			// Panel should go out...
			menuController.WillHideSettingsPanel ();
			yield return (Scroll (_rectTransform, inPosition, _midPosition, scrollHorizontalDuration));
			yield return (Scroll (_rectTransform, _midPosition, outPosition, scrollVerticalDuration));
			_isShowing = false;
		}

		_isAnimating = false;
		yield break;
	}

	private IEnumerator Scroll (RectTransform rectTransform, Vector3 localStartPosition, Vector3 localTargetPosition, float duration) {
		float timeStart = Time.time;
		float u;
		Vector3 currentPos;

		while ((Time.time - timeStart) < duration) {
			u = (Time.time - timeStart) / duration;
			currentPos = Vector3.Lerp (localStartPosition, localTargetPosition, u);
			rectTransform.anchoredPosition = currentPos;
			yield return null;
		}

		rectTransform.anchoredPosition = localTargetPosition;
	}

}
