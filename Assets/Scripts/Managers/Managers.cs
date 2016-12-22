using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioManager))]
[RequireComponent (typeof (ScoreManager))]
[RequireComponent (typeof (LivesManager))]
[RequireComponent (typeof (AdsManager))]
[RequireComponent (typeof (StatsManager))]
[RequireComponent (typeof (UnlockablesManager))]
[RequireComponent (typeof (LanguageManager))]
public class Managers : MonoBehaviour {

	private static Managers Manager;
	public static AudioManager Audio;
	public static ScoreManager Score;
	public static LivesManager Lives;
	public static AdsManager Ads;
	public static StatsManager Stats;
	public static UnlockablesManager Unlockables;
	public static LanguageManager Language;

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
		Stats = GetComponent<StatsManager> ();
		Unlockables = GetComponent<UnlockablesManager> ();
		Language = GetComponent<LanguageManager> ();


		if (killData) {
			killData = false;
			KillData ();
		}
	}

	public void KillData () {
		DataManager.DeleteData ();
		Application.Quit ();

		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}

}
