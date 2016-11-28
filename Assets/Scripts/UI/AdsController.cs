using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour {

	public GameObject watchAdPrompt;
	public GameObject afterAdPrompt;


	void OnEnable () {
		watchAdPrompt.SetActive (true);
		afterAdPrompt.SetActive (false);
	}


	public void OnWatchAd () {
		ShowAd ();
	}


	public void OnCloseWatchAddPrompt () {
		watchAdPrompt.SetActive (false);
		gameObject.SetActive (false);
	}


	public void OnCloseAfterAdPrompt () {
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
