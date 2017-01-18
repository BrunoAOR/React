using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LivesManager : MonoBehaviour {

	public int maxLivesCount = 3;
	public float minutesToNewLife = 1;

	[Header ("References")]
	public Text livesCount;
	public GameObject timer;
	public Text livesTimer;

	private int currentLivesCount = 5;
	private System.DateTime timeForNextLife;


	void Awake () {
		LoadData ();
	}


	private void SaveData () {
		LivesData livesData = new LivesData ();
		livesData.currentLivesCount = currentLivesCount;
		livesData.timeForNextLife = timeForNextLife;
		DataManager.Save_LivesData (livesData);
	}


	private void LoadData () {
		LivesData livesData = DataManager.Load_LivesData ();
		currentLivesCount = Mathf.Clamp(livesData.currentLivesCount, 0, maxLivesCount);
		timeForNextLife = livesData.timeForNextLife;
	}


	void Start () {
		UpdateLifeCount ();
		UpdateTimer ();
	}


	void Update () {
		if (currentLivesCount == maxLivesCount)
			return;

		UpdateLifeCount ();
		UpdateTimer ();
	}


	public bool UseLife () {
		if (currentLivesCount > 0) {
			if (currentLivesCount == maxLivesCount) {
				timeForNextLife = System.DateTime.UtcNow.AddMinutes (minutesToNewLife);
			}
			currentLivesCount--;
			SaveData ();
			return true;
		} else {
			return false;
		}
	}


	public int GetLivesCount () {
		return (currentLivesCount);
	}

	public bool AddLife () {
		if (currentLivesCount < maxLivesCount) {
			currentLivesCount++;
			SaveData ();
			return true;
		} else {
			return false;
		}
	}


	public void FillLives () {
		currentLivesCount = maxLivesCount;
		timeForNextLife = System.DateTime.MaxValue;
		SaveData ();
		UpdateLifeCount ();
		UpdateTimer ();
	}


	private void UpdateLifeCount () {
		if (currentLivesCount >= maxLivesCount) {
			currentLivesCount = maxLivesCount;
			timeForNextLife = System.DateTime.MaxValue;
		} else {
			if (System.DateTime.UtcNow > timeForNextLife) {
				timeForNextLife = timeForNextLife.AddMinutes (minutesToNewLife);
				AddLife ();
				UpdateLifeCount ();
			}
		}
		SetLifeLabel ();
	}


	private void SetLifeLabel () {
		livesCount.text = currentLivesCount.ToString ();
	}


	private void UpdateTimer () {
		if (timeForNextLife != System.DateTime.MaxValue) {
			TurnOnTimer ();
			System.TimeSpan timeSpan = timeForNextLife - System.DateTime.UtcNow;
			SetTimerLabel (timeSpan.Minutes, timeSpan.Seconds);
		} else {
			TurnOffTimer ();
		}
	}


	private void SetTimerLabel (int minutes, int seconds) {
		livesTimer.text = minutes.ToString ("D") + ":" + seconds.ToString ("D2");
	}


	private void TurnOnTimer () {
		timer.gameObject.SetActive (true);
	}


	private void TurnOffTimer () {
		SetTimerLabel (0, 0);
		timer.gameObject.SetActive (false);
	}

}


[System.Serializable]
public class LivesData {
	
	public int currentLivesCount = int.MaxValue;
	public System.DateTime timeForNextLife = System.DateTime.MaxValue;
}