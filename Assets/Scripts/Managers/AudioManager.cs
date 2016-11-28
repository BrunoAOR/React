using UnityEngine;
using System.Collections;

// Warning CS0649: Field '#fieldname#' is never assigned to, and will always have its default value null (CS0649) (Assembly-CSharp)
// Warning was raised for the following fields: musicSource1, musicSource2 and soundSource
// Warning was disabled because these private fields are serialized and assigned through the inspector
#pragma warning disable 0649

public class AudioManager : MonoBehaviour {

	[Header ("Audio Sources")]
	[SerializeField]
	private AudioSource	musicSource1;
	[SerializeField]
	private AudioSource	musicSource2;
	[SerializeField]
	private AudioSource	soundSource;

	[Header ("Music crossfadding")]
	public float		crossFadeDuration = 1;
	private bool		_crossFading;
	private AudioSource	_activeMusic;
	private AudioSource	_inactiveMusic;

	[Header ("Sound clips")]
	public string		musicClip1;
	public string		musicClip2;

	public AudioClip[]	SFX;

	public bool musicMute {
		get{
			if (musicSource1 != null) {
				return (musicSource1.mute);
			}
			return false;
		}
		set {
			if (musicSource1 != null) {
				musicSource1.mute = value;
				musicSource2.mute = value;
			}
		}
	}


	private float	_musicVolume;
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


	public bool soundMute {
		get {
			return (AudioListener.pause);
		}
		set {
			AudioListener.pause = value;
		}
	}


	private float	_soundVolume;
	public float soundVolume {
		get {
			return (_soundVolume);
		}
		set {
			_soundVolume = value;
			AudioListener.volume = _soundVolume;
		}
	}
		

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


	public void PlaySFX (SFX sfx) {
		PlaySFX ((int) sfx
		);
	}

	public void PlaySFX (int sfxIdx) {
		if (sfxIdx < 0 || sfxIdx >= SFX.Length) {
			return;
		}
		PlaySound (SFX [sfxIdx]);
	}

	public void PlaySound (AudioClip clip) {
		soundSource.PlayOneShot (clip);
	}

	public void PlayMusic1 () {
		PlayMusic (Resources.Load("Music/" + musicClip1) as AudioClip);
	}

	public void PlayMusic2 () {
		PlayMusic (Resources.Load("Music/" + musicClip2) as AudioClip);
	}


	private void PlayMusic (AudioClip clip) {
		if (!_crossFading) {
			StartCoroutine ( FadeToMusic (clip) );
		}
	}


	private IEnumerator FadeToMusic (AudioClip clip) {
		// Activate Flag to prevent any further calls to FadeTo-Coroutines while they run
		_crossFading = true;

		// Assign the clip to the _inactiveMusic
		_inactiveMusic.clip = clip;

		// Set the _inactiveMusic volume to zero and start playing
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
