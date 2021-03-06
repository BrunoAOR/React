﻿using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Warning CS0649: Field '#fieldname#' is never assigned to, and will always have its default value null (CS0649) (Assembly-CSharp)
// Warning was raised for the following fields: musicSource1, musicSource2, soundSource and loudSoundSource
// Warning was disabled because these private fields are serialized and assigned through the inspector
#pragma warning disable 0649


public enum SFX : int {
	ButtonsOn,
	ButtonUnlit,
	IconClicked_Locked,
	IconClicked_NewSection,
	MenuButton,
	MenuButton_GO,
	PauseButton,
	PausePanelButton,
	CountDown,
	CountDownEnd,
	TapPrompt,
	ScoreCounting,
	Test
}


public class AudioManager : MonoBehaviour {

	[Header ("Audio Mixer")]
	public AudioMixerSnapshot defaultSnapshot;
	public AudioMixerSnapshot noMusicSnapshot;
	public AudioMixerSnapshot noSoundSnapshot;
	public AudioMixerSnapshot allMutedSnapshot;
	public float normalTransitionTime = 0.5f;

	[Header ("Audio Sources")]
	[SerializeField]
	private AudioSource	musicSource1;
	[SerializeField]
	private AudioSource	musicSource2;
	[SerializeField]
	private AudioSource	soundSource;
	[SerializeField]
	private AudioSource loudSoundSource;

	[Header ("Music crossfadding")]
	public float		crossFadeDuration = 1;
	private bool		_crossFading;
	private AudioSource	_activeMusic;
	private AudioSource	_inactiveMusic;

	[Header ("Sound clips")]
	public string		musicClip1;
	public float		pitch1 = 1f;
	public string		musicClip2;
	public float		pitch2 = 1f;

	public AudioClip[]	SFX;

	public bool musicMute {
		get{
			return (_musicMute);
		}
		set {
			_musicMute = value;
			_instantSnapshotChange = false;
			UpdateSnapshot ();
		}
	}
	private bool _musicMute;


	public float musicVolume {
		get {
			return (_musicVolume);
		}
		set {
			_musicVolume = value;
			if (musicSource1 != null) {
				musicSource1.volume = _musicVolume;
				musicSource2.volume = _musicVolume;
			}
		}
	}
	private float _musicVolume;

	public bool soundMute {
		get {
			return (_soundMute);
		}
		set {
			_soundMute = value;
			_instantSnapshotChange = true;
			UpdateSnapshot ();
		}
	}
	private bool _soundMute;

	private bool _instantSnapshotChange;

	public float soundVolume {
		get {
			return (_soundVolume);
		}
		set {
			_soundVolume = value;
			AudioListener.volume = _soundVolume;
		}
	}
	private float	_soundVolume;


	void Awake () {
		musicSource1.loop = true;
		musicSource1.ignoreListenerPause = true;
		musicSource1.ignoreListenerVolume = true;
		musicSource2.loop = true;
		musicSource2.ignoreListenerPause = true;
		musicSource2.ignoreListenerVolume = true;

		_activeMusic = musicSource2;
		_inactiveMusic = musicSource1;

		soundVolume = 1f;
		musicVolume = 1f;

	}

	private void UpdateSnapshot () {
		float transitionTime = _instantSnapshotChange ? 0f : normalTransitionTime;

		if (_musicMute) {
			// Music OFF
			if (_soundMute) {
				// Sound OFF
				allMutedSnapshot.TransitionTo (transitionTime);
			} else {
				// Sound ON
				noMusicSnapshot.TransitionTo (transitionTime);
			}
		} else {
			// Music ON
			if (_soundMute) {
				// Sound OFF
				noSoundSnapshot.TransitionTo (transitionTime);
			} else {
				// Sound ON
				defaultSnapshot.TransitionTo (transitionTime);
			}
		}
	}


	public void PlaySFX (SFX sfx, bool loop = false, bool loud = false) {
		PlaySFX ((int) sfx, loop);
	}

	public void PlaySFX (int sfxIdx, bool loop = false, bool loud = false) {
		if (sfxIdx < 0 || sfxIdx >= SFX.Length) {
			return;
		}
		if (loop) {
			LoopSound (SFX [sfxIdx], loud);
		} else {
			PlaySound (SFX [sfxIdx], loud);
		}
	}

	public void PlaySound (AudioClip clip, bool loud = false) {
		if (loud) {
			loudSoundSource.PlayOneShot (clip);
		} else {
			soundSource.PlayOneShot (clip);
		}
	}

	public void LoopSound (AudioClip clip, bool loud = false) {
		if (loud) {
			loudSoundSource.loop = true;
			loudSoundSource.clip = clip;
			loudSoundSource.Play ();
		} else {
			soundSource.loop = true;
			soundSource.clip = clip;
			soundSource.Play ();
		}
	}

	public void StopLoop (bool allowClipToFinish = false, bool wasLoud = false) {
		if (wasLoud) {
			loudSoundSource.loop = false;
			if (!allowClipToFinish) {
				loudSoundSource.Stop ();
			}
		} else {
			soundSource.loop = false;
			if (!allowClipToFinish) {
				soundSource.Stop ();
			}
		}
	}

	public void PlayMusic1 () {
		PlayMusic (Resources.Load("Music/" + musicClip1) as AudioClip, pitch1);
	}

	public void PlayMusic2 () {
		PlayMusic (Resources.Load("Music/" + musicClip2) as AudioClip, pitch2);
	}


	private void PlayMusic (AudioClip clip, float pitch = 1f) {
		if (!_crossFading) {
			StartCoroutine ( FadeToMusic (clip, pitch) );
		}
	}


	private IEnumerator FadeToMusic (AudioClip clip, float pitch) {
		// Activate Flag to prevent any further calls to FadeTo-Coroutines while they run
		_crossFading = true;

		// Assign the clip to the _inactiveMusic
		_inactiveMusic.clip = clip;

		// Set the _inactiveMusic volume to zero and start playing
		_inactiveMusic.pitch = pitch;
		_inactiveMusic.volume = 0;
		_inactiveMusic.Play ();

		// Calculate the crossFadeRate and scale it to account for the actual starting volume
		float crossFadeRate = 1 / crossFadeDuration;
		float scaledRate = crossFadeRate * _musicVolume;

		// Decrease the _activeMusic volume while increasing the _inactiveMusic volume over many frames.
		while (_activeMusic.volume > 0) {
			_activeMusic.volume -= scaledRate * Time.deltaTime;
			_inactiveMusic.volume += scaledRate * Time.deltaTime;
			yield return null;
		}

		// switch _activeMusic and _inactiveMusic
		AudioSource _tAS = _activeMusic;
		_activeMusic = _inactiveMusic;
		_inactiveMusic = _tAS;

		// Readjust the now _activeMusic volume (to ensure it lands in exactly MusicVolume) and stop playback on _inactiveMusic
		_activeMusic.volume = musicVolume;
		_inactiveMusic.Stop ();

		// Deactive flag to allow any new calls to FadeTo-Coroutines
		_crossFading = false;
	}


	public void	StopMusic () {
		if (!_crossFading) {
			StartCoroutine ( FadeToSilence () );
		}
	}


	private IEnumerator FadeToSilence () {
		// Activate Flag to prevent any further calls to FadeTo-Coroutines while they run
		_crossFading = true;

		// Calculate the crossFadeRate and scale it to account for the actual starting volume
		float crossFadeRate = 1 / crossFadeDuration;
		float scaledRate = crossFadeRate * _musicVolume;

		// Decrease the _activeMusic volume over many frames.
		while (_activeMusic.volume > 0) {
			_activeMusic.volume -= scaledRate * Time.deltaTime;
			yield return null;
		}

		// switch _activeMusic and _inactiveMusic
		AudioSource _tAS = _activeMusic;
		_activeMusic = _inactiveMusic;
		_inactiveMusic = _tAS;

		// Stop playbakc and readjust the _activeMusic volume back to _musicVolume to allow for smooth music restart
		_activeMusic.Stop ();
		_activeMusic.volume = _musicVolume;

		// Deactive flag to allow any new calls to FadeTo-Coroutines
		_crossFading = false;
	}

	private void OnValidate () {
		if (crossFadeDuration < 0) {
			crossFadeDuration = 0;
		}
	}

}
