using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioManager))]
[RequireComponent (typeof (ScoreManager))]
[RequireComponent (typeof (LivesManager))]
[RequireComponent (typeof (AdsManager))]
public class Managers : MonoBehaviour {

	private static Managers Manager;
	public static AudioManager Audio;
	public static ScoreManager Score;
	public static LivesManager Lives;
	public static AdsManager Ads;

	public bool killData = false;

	void Awake () {
		// Manking sure there is only one Managers GameObject
		if (Manager == null) {
			Manager = this;
			DontDestroyOnLoad (this.gameObject);
		} else if (Manager != this) {
			Destroy (gameObject);
		}

		// Set up static references
		Audio = GetComponent<AudioManager> ();
		Score = GetComponent<ScoreManager> ();
		Lives = GetComponent<LivesManager> ();
		Ads = GetComponent<AdsManager> ();


		if (killData) {
			killData = false;
			DataManager.DeleteData ();
		}
	}

}
