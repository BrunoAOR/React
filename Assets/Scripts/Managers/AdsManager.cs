using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;


public class AdsManager : MonoBehaviour {

	public string zoneId;


	public void ShowAd () {
		ShowAd (zoneId, null);
	}

	public bool IsReady (string zoneId = null) {
		return Advertisement.IsReady (zoneId);
	}


	public void ShowAd (string zoneId, System.Action<ShowResult> ResultHandler) {

		#if UNITY_EDITOR
			StartCoroutine (EditorWaitForAd ());
		#endif


		if (string.IsNullOrEmpty (zoneId)) {
			if (!string.IsNullOrEmpty (this.zoneId)) {
				zoneId = this.zoneId;
			} else {
				zoneId = null;
			}
		}

		ShowOptions options = new ShowOptions ();
		if (ResultHandler == null) {
			options.resultCallback = HandleShowResult;
		} else {
			options.resultCallback = ResultHandler;
		}

		if (Advertisement.IsReady(zoneId)) {
			Advertisement.Show (zoneId, options);
		} else {
			HandleAdNotReady ();
		}
	}


	private IEnumerator EditorWaitForAd () {
		float currentTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		yield return null;

		while (Advertisement.isShowing) {
			yield return null;
		}

		Time.timeScale = currentTimeScale;
	}


	private void HandleShowResult (ShowResult result) {
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

	
	private void HandleAdNotReady () {
		Debug.Log ("Ads are not ready to be shown.");
	}


	private void HandleAdFinished () {
		Debug.Log ("Ad completed. Should reward player.");
	}


	private void HandleAdSkipped () {
		Debug.Log ("Ad was skipped.");
	}


	private void HandleAdFailed () {
		Debug.Log ("Ad failed to show.");
	}
}
