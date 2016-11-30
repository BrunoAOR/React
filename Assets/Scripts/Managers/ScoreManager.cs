using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {

	public Text scoreText;
	public PointsSpawner pointsSpawner;
	public CameraShaker cameraShaker;
	public int defaultPointsPerButton = 100;

	[Header ("Text Animation")]
	[Range (0f, 0.5f)]
	public float animationDuration = 0.1f; 
	public Color possitivePointsColor;
	[Range (0.5f, 1.5f)]
	public float positivePointsScaling = 1.1f;
	public Color negativePointsColor;
	[Range (0.5f, 1.5f)]
	public float negativePointsScaling = 0.9f;

	private int score;
	private int multiplier = 1;
	private int originalTextSize;
	private Color originalTextColor;
	private IEnumerator currentAnimation;

	private Highscores highscores;

	void Awake () {
		LoadData ();
	}


	private void LoadData () {
		highscores = DataManager.Load_Highscores ();
		if (highscores == null) {
			highscores = new Highscores ();
		}
	}


	void Start () {
		originalTextSize = scoreText.fontSize;
		originalTextColor = scoreText.color;
		score = 0;
		SetScoreText ();
	}


	public int AddPoints () {
		return (AddPoints (defaultPointsPerButton));
	}


	public int AddPoints (int points) {
		if (points == 0)
			return (0);

		if (points < 0)
			ResetMultiplier ();

		int pointsAdded = points * multiplier;

		score += pointsAdded;

		if (points > 0)
			multiplier++;

		SetScoreText ();
		if (currentAnimation != null) {
			StopCoroutine (currentAnimation);
		}
		currentAnimation = AnimateScore (points > 0);
		StartCoroutine (currentAnimation);

		return pointsAdded;
	}


	public void AnimateAddPoints (Vector2 source) {
		AnimateAddPoints (defaultPointsPerButton, source);
	}


	public void AnimateAddPoints (int points, Vector2 source) {
		int pointsAdded = AddPoints (points);
		Vector2 target = scoreText.rectTransform.position;
		target = Camera.main.ScreenToViewportPoint (target);

		pointsSpawner.AnimatePoints (pointsAdded.ToString(), source, target);
	}


	public void ResetScore () {
		score = 0;
		SetScoreText ();
		ResetMultiplier ();
	}


	public void ResetMultiplier () {
		multiplier = 1;
	}


	public int GetScore () {
		return score;
	}


	public bool SetHighscore (GameModeLogic gameMode, int gridSize, ButtonsBehaviour[] behaviours) {
		string gameModeName = gameMode.modeName;

		bool newHighscore =	highscores.SetHighscore (score, gameModeName, gridSize, behaviours);

		if (newHighscore) {
			DataManager.Save_Highscores (highscores);
		}

		return (newHighscore);
		
	}



	public int GetHighscore (GameModeLogic gameMode, int gridSize, ButtonsBehaviour[] behaviours) {
		string gameModeName = gameMode.modeName;

		int storedHighscore = highscores.GetHighscore (gameModeName, gridSize, behaviours);

		return (storedHighscore);
	}


	public void SetHighscores (Highscores loadedHighscores) {
		highscores = loadedHighscores;
	}


	private void SetScoreText () {
		scoreText.text = score.ToString ();
	}


	private IEnumerator AnimateScore (bool possitiveScoreChange) {
		float timeStart = Time.time;
		float scaleFactor = possitiveScoreChange ? positivePointsScaling : negativePointsScaling;
		int startSize = (int)(originalTextSize * scaleFactor);
		Color startColor = possitiveScoreChange ? possitivePointsColor : negativePointsColor;
		scoreText.fontSize = startSize;

		while ((Time.time - timeStart) < animationDuration) {
			float u = (Time.time - timeStart) / animationDuration;
			float textSize = (1 - u) * startSize + u * originalTextSize;
			scoreText.fontSize = (int)textSize;
			scoreText.color = Color.Lerp (startColor, originalTextColor, u);
			yield return null;
		}
		currentAnimation = null;
	}



}
