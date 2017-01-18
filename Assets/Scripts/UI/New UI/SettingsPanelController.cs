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
	public GameObject PanelRim;
	public Image musicToggle;
	public Image soundToggle;

	[HideInInspector]
	public MenuController menuController;

	private RectTransform _rectTransform;
	private Vector3 _midPosition;

	private bool _interactable {
		get {
			return (menuController.CanShowSettingsPanel ());
		}
	}
	private bool _isAnimating = false;
	private bool _isShowing = false;

	void Awake () {
		_rectTransform = GetComponent<RectTransform> ();
		_midPosition = new Vector3 (outPosition.x, inPosition.y, 0);
		_rectTransform.anchoredPosition = outPosition;
		PanelRim.SetActive (true);

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
			_musicEnabled = true;
			soundToggle.color = enabledColor;
		}

		// TODO: Get the actual saved selectedLanguage from the LanguageManager
		_selectedLanguageIndex = 0;
		ToggleToFlag (_selectedLanguageIndex);
	}

	public void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	public void OnSettingsIconClicked () {
		if (!_interactable || _isAnimating) {
			return;
		}
		StartCoroutine (ShowHidePanel ());
	}

	public void OnMusicClicked () {
		if (!_interactable || _isAnimating) {
			return;
		}

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
		}
	}

	public void OnFlagClicked (int index) {
		if (!_interactable || _isAnimating) {
			return;
		}

		ToggleToFlag (index);
	}

	private void ToggleToFlag (int flagIndex) {
		if (flagIndex < 0 || flagIndex >= flagRims.Length) {
			return;
		}

		for (int i = 0; i < flagRims.Length; i++) {
			flagRims [i].SetActive (false);
		}
		flagRims [flagIndex].SetActive (true);

		// TODO: Here, we should tell the manager to change language.
		Debug.Log ("Language at index " + flagIndex + " selected!");

		_selectedLanguageIndex = flagIndex;
	}

	private IEnumerator ShowHidePanel () {
		_isAnimating = true;

		if (!_isShowing) {
			// Panel should go in...
			PanelRim.SetActive (false);
			menuController.WillShowSettingsPanel ();
			yield return (Scroll (_rectTransform, outPosition, _midPosition, scrollVerticalDuration));
			yield return (Scroll (_rectTransform, _midPosition, inPosition, scrollHorizontalDuration));
			_isShowing = true;
		} else {
			// Panel should go out...
			yield return (Scroll (_rectTransform, inPosition, _midPosition, scrollHorizontalDuration));
			menuController.WillHideSettingsPanel ();
			yield return (Scroll (_rectTransform, _midPosition, outPosition, scrollVerticalDuration));
			PanelRim.SetActive (true);
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
