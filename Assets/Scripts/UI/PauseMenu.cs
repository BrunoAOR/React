using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	
	public GameObject pausePanel;
	public GameObject pauseCollider;
	public CountDownUIText countDown;
	public ColorBlenderUIGraphic coverImage;

	private float originalTimeScale;
	private bool _isPaused;
	private bool _buttonsClickable;


	private void Awake () {
		pausePanel.SetActive (false);
		_isPaused = false;
		_buttonsClickable = true;
	}


	private void OnEnable ()  {
		coverImage.gameObject.SetActive (false);
	}


	public void OnPauseButtonClicked () {
		if (!_buttonsClickable)
			return;
		
		if (_isPaused) {
			Managers.Audio.PlaySFX (SFX.PausePanelButton);
			StartCoroutine (Unpause () );
		} else {
			Managers.Audio.PlaySFX (SFX.PauseButton);
			Pause ();
		}
	}


	public void OnResume () {
		if (!_buttonsClickable)
			return;

		// Checking just in case something went really bad
		if (_isPaused) {
			Managers.Audio.PlaySFX (SFX.PausePanelButton);
			StartCoroutine (Unpause () );
		}
	}


	private void Pause () {
		_isPaused = true;
		originalTimeScale = Time.timeScale;
		Time.timeScale = 0;

		pauseCollider.SetActive (true);
		coverImage.gameObject.SetActive (true);


		pausePanel.SetActive (true);

		RoundManager.S.Pause ();
	}


	private IEnumerator Unpause () {
		Time.timeScale = originalTimeScale;

		RoundManager.S.UnpausingStarted ();

		pausePanel.SetActive (false);

		_buttonsClickable = false;

		yield return (StartCoroutine (countDown.StartCountDown() ) );
		coverImage.gameObject.SetActive (false);
		pauseCollider.SetActive (false);

		_buttonsClickable = true;
		_isPaused = false;

		RoundManager.S.Unpause ();
	}


	public void OnQuitGame () {
		if (!_buttonsClickable)
			return;

		Managers.Audio.PlaySFX (SFX.PausePanelButton);
		_buttonsClickable = false;
		StartCoroutine (QuitGame ());
	}


	private IEnumerator QuitGame () {
		Time.timeScale = originalTimeScale;
		yield return (coverImage.StartColorBlend ());
		pausePanel.SetActive (false);
		pauseCollider.SetActive (false);
		_buttonsClickable = true;
		_isPaused = false;
		yield return new WaitForSeconds (1.0f);
		coverImage.gameObject.SetActive (false);
		RoundManager.S.QuitRound ();
	}

}
