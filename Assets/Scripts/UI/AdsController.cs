using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour {

	public GameObject noAdsPrompt;
	public GameObject noLivesPrompt;
	public GameObject watchAdPrompt;
	public GameObject afterAdPrompt;

	public MenuControllerBase menuController;

	void OnEnable () {
		noAdsPrompt.SetActive (false);
		noLivesPrompt.SetActive (false);
		watchAdPrompt.SetActive (false);
		afterAdPrompt.SetActive (false);

		if (Managers.Ads.IsReady ()) {
			watchAdPrompt.SetActive (true);
		} else {
			Debug.Log ("Ads are NOT ready to be shown!");
			if (Managers.Lives.GetLivesCount () <= 0) {
				noLivesPrompt.SetActive (true);
			} else {
				noAdsPrompt.SetActive (true);
			}
		}

	}

	void OnDisable () {
		menuController.AdsFinished ();
	}

	public void OnWatchAd () {
		Managers.Audio.PlaySFX (SFX.MenuButton);
		ShowAd ();
	}


	public void OnCloseNoAdsPrompt () {
		noAdsPrompt.SetActive (false);
		gameObject.SetActive (false);
	}


	public void OnCloseNoLivesPrompt () {
		Managers.Audio.PlaySFX (SFX.MenuButton);
		noLivesPrompt.SetActive (false);
		gameObject.SetActive (false);
	}


	public void OnCloseWatchAddPrompt () {
		Managers.Audio.PlaySFX (SFX.MenuButton);
		watchAdPrompt.SetActive (false);
		gameObject.SetActive (false);
	}


	public void OnCloseAfterAdPrompt () {
		Managers.Audio.PlaySFX (SFX.MenuButton);
		afterAdPrompt.SetActive (false);
		gameObject.SetActive (false);
	}


	public void ShowAd () {
		Managers.Ads.ShowAd (null, AdResultHandler);
	}


	private void AdResultHandler (ShowResult result) {
		switch (result) {
		case ShowResult.Finished:
			HandleAdFinished ();
			break;
		case ShowResult.Skipped:
			HandleAdSkipped ();
			break;
		case ShowResult.Failed:
			HandleAdFailed ();
			break;
		}
	}


	private void HandleAdFinished () {
		Debug.Log ("Ad completed. Should reward player.");
		watchAdPrompt.SetActive (false);

		Managers.Lives.FillLives ();

		afterAdPrompt.SetActive (true);
	}


	private void HandleAdSkipped () {
		Debug.Log ("Ad was skipped.");
	}


	private void HandleAdFailed () {
		Debug.Log ("Ad failed to show.");
	}
}
