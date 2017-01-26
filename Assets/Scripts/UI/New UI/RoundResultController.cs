using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundResultController : MonoBehaviour {

	public AudioClip[] clips;

	[Header ("Animation parameters")]
	public float initialDelay = 0.5f;
	public float stepWaitTime = 0.5f;
	public float panelEntryDuration = 0.5f;
	public float modeAndDifficultyEntryDuration = 0.25f;
	public float separatorEntryDuration = 0.25f;
	public float scoresEntryDuration = 0.5f;
	public int scoreCountedPerSecond = 5000;

	private WaitForSeconds _initialDelay;
	private WaitForSeconds _waitTime;

	[Header ("References")]
	public GameObject roundResultPanel;
	public Text gameModeText;
	public Text difficultyText;
	public GameObject scoreSection;
	public Image separator;
	public Text highscoreText;
	public Text currentScoreText;
	public GameObject newHighscoreLabel;
	public GameObject tapPrompt;

	private Vector2 _canvasReferenceResolution;
	private int _currentScore;
	private int _previousHighscore;
	private bool _newHighscore;
	private int _elementsCount = 9;
	private Vector3[] _elementsLocalPositions;
	private IEnumerator _showRoundResultCoroutine;
	private IEnumerator[] _showRoundResultInnerCoroutines = new IEnumerator[2];
	private ZeroToNumberTyper _numberTyper;

	void Awake () {
		_canvasReferenceResolution = GetComponentInParent<CanvasScaler> ().referenceResolution;
		_elementsLocalPositions = new Vector3[_elementsCount];
		_initialDelay = new WaitForSeconds (initialDelay);
		_waitTime = new WaitForSeconds (stepWaitTime);
		_numberTyper = new ZeroToNumberTyper ();
		RecordElementsLocalPositions ();
	}

	public void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	public void OnClick () {
		if (_showRoundResultCoroutine != null) {
			// So, animation is still running...
			SkipAnimations ();
		} else {
			// So, the animation has already finished...

			Managers.Audio.PlaySFX (SFX.TapPrompt);

			SetActive (false);
			RoundManager.S.GoToMenu ();
		}
	}

	private void SkipAnimations () {
		// Stop main coroutine
		StopCoroutine (_showRoundResultCoroutine);
		_showRoundResultCoroutine = null;

		// Stop any internal coroutines that may be running;
		for (int i = 0; i < _showRoundResultInnerCoroutines.Length; i++) {
			if (_showRoundResultInnerCoroutines [i] == null) {
				break;
			} else {
				StopCoroutine (_showRoundResultInnerCoroutines [i]);
				_showRoundResultInnerCoroutines [i] = null;
			}
		}

		_numberTyper.StopCounter ();

		// Write in the score (might have stopped mid-count) and stop the looping sound
		currentScoreText.text = _currentScore.ToString ();
		Managers.Audio.StopLoop ();

		// Put everythin in place
		ApplyElementsLocalPositions ();

		// Turn everything on
		SetElementsState (true);

		// Turn the highscore label off if previous highscore wasn't beaten
		if (!_newHighscore) {
			newHighscoreLabel.gameObject.SetActive (false);
		}
	}

	public void ShowRoundResult (GameMode gameMode, Difficulty difficulty, int score) {
		SetActive (true);

		// Register the round in the StatsManager
		Managers.Stats.RegisterPlay (gameMode, difficulty, score);

		// Record previous Highscore, in case it changes
		_previousHighscore = Managers.Score.GetHighscore (gameMode, difficulty);

		// Update Highscore in the Score manager and records if a new highscore was achieved
		_newHighscore = Managers.Score.SetHighscore (gameMode, difficulty);

		// Record current score, to be used in case the coroutine is stopped and in the coroutine itself
		_currentScore = score;

		if (_showRoundResultCoroutine != null) {
			StopCoroutine (_showRoundResultCoroutine);
		}
		_showRoundResultCoroutine = ShowRoundResultCoroutine (gameMode, difficulty);
		StartCoroutine (_showRoundResultCoroutine);
	}

	private IEnumerator ShowRoundResultCoroutine (GameMode gameMode, Difficulty difficulty) {
		SetElementsState (false);

		// Set up labels
		gameModeText.text = string.Format ("{0} mode", gameMode.ToString ());
		difficultyText.text = difficulty.ToString ();
		highscoreText.text = _previousHighscore.ToString ();
		currentScoreText.text = "";

		// Initial delay
		yield return (_initialDelay);

		// Animate the panel background entry (downwards)
		roundResultPanel.gameObject.SetActive (true);
		Vector3 panelInPos = roundResultPanel.transform.localPosition;
		Vector3 panelOutPos = panelInPos;
		panelOutPos.y += _canvasReferenceResolution.y;

		Managers.Audio.PlaySound (clips [0], true);
		_showRoundResultInnerCoroutines [0] = ScrollIn (roundResultPanel.gameObject, panelOutPos, panelInPos, panelEntryDuration);
		yield return (_showRoundResultInnerCoroutines [0]);
		_showRoundResultInnerCoroutines [0] = null;

		// Wait before the next step
		yield return (_waitTime);

		// Animate the gameModeText (downwards) and the dificultyText (upwards) entries
		gameModeText.gameObject.SetActive (true);
		difficultyText.gameObject.SetActive (true);

		Vector3 modeInPos = gameModeText.rectTransform.localPosition;
		Vector3 modeOutPos = modeInPos;
		modeOutPos.y += gameModeText.rectTransform.sizeDelta.y;

		Vector3 diffInPos = difficultyText.rectTransform.localPosition;
		Vector3 diffOutPos = diffInPos;
		diffOutPos.y -= difficultyText.rectTransform.sizeDelta.y;

		Managers.Audio.PlaySound (clips [1], true);
		_showRoundResultInnerCoroutines [0] = ScrollIn (gameModeText.gameObject, modeOutPos, modeInPos, modeAndDifficultyEntryDuration);
		_showRoundResultInnerCoroutines [1] = ScrollIn (difficultyText.gameObject, diffOutPos, diffInPos, modeAndDifficultyEntryDuration);
		StartCoroutine (_showRoundResultInnerCoroutines [0]);
		yield return (_showRoundResultInnerCoroutines [1]);
		_showRoundResultInnerCoroutines [0] = null;
		_showRoundResultInnerCoroutines [1] = null;

		// Show the score label
		yield return (_waitTime);
		scoreSection.SetActive (true);

		// Animate the separator entry
		separator.gameObject.SetActive (true);
		Vector3 inPos = separator.rectTransform.localPosition;
		Vector3 outPos = inPos;
		outPos.y += separator.rectTransform.sizeDelta.y;

		Managers.Audio.PlaySound (clips [2], true);
		_showRoundResultInnerCoroutines [0] = ScrollIn (separator.gameObject, outPos, inPos, separatorEntryDuration);
		yield return (_showRoundResultInnerCoroutines [0]);
		_showRoundResultInnerCoroutines [0] = null;

		// Animate entry from highscoreText (contains highscoreLabel) and currentScoreText (contains currentScoreLabel)
		highscoreText.gameObject.SetActive (true);
		currentScoreText.gameObject.SetActive (true);
		Vector3 leftPos = highscoreText.rectTransform.localPosition;
		Vector3 rightPos = currentScoreText.rectTransform.localPosition;

		Managers.Audio.PlaySound (clips [3], true);
		_showRoundResultInnerCoroutines [0] = ScrollIn (highscoreText.gameObject, rightPos, leftPos, scoresEntryDuration);
		_showRoundResultInnerCoroutines [1] = ScrollIn (currentScoreText.gameObject, leftPos, rightPos, scoresEntryDuration);
		StartCoroutine (_showRoundResultInnerCoroutines [0]);
		yield return (_showRoundResultInnerCoroutines [1]);
		_showRoundResultInnerCoroutines [0] = null;
		_showRoundResultInnerCoroutines [1] = null;

		Managers.Audio.PlaySFX (SFX.ScoreCounting, true);
		_showRoundResultInnerCoroutines [0] = _numberTyper.StartCounter (currentScoreText, 0, _currentScore, _currentScore / (float)scoreCountedPerSecond);
		yield return (StartCoroutine(_showRoundResultInnerCoroutines[0]));
		_showRoundResultInnerCoroutines [0] = null;
		Managers.Audio.StopLoop ();

		// Show the newHighscoreLabel if a new highscore was achieved
		yield return (_waitTime);
		if (_newHighscore) {
			newHighscoreLabel.gameObject.SetActive (true);
		}

		// Show the tap to continue label
		tapPrompt.SetActive (true);

		// Coroutine finished
		_showRoundResultCoroutine = null;
	}

	private IEnumerator ScrollIn (GameObject gameObject, Vector3 localStartPosition, Vector3 localTargetPosition, float duration) {
		float timeStart = Time.time;
		float u;
		Vector3 currentPos;

		while ((Time.time - timeStart) < duration) {
			u = (Time.time - timeStart) / duration;
			currentPos = Vector3.Lerp (localStartPosition, localTargetPosition, u);
			gameObject.transform.localPosition = currentPos;
			yield return null;
		}

		gameObject.transform.localPosition = localTargetPosition;
	}

	private void SetElementsState (bool activeState) {
		roundResultPanel.gameObject.SetActive (activeState);
		gameModeText.gameObject.SetActive (activeState);
		difficultyText.gameObject.SetActive (activeState);
		scoreSection.gameObject.SetActive (activeState);
		separator.gameObject.SetActive (activeState);
		highscoreText.gameObject.SetActive (activeState);
		currentScoreText.gameObject.SetActive (activeState);
		newHighscoreLabel.gameObject.SetActive (activeState);
		tapPrompt.gameObject.SetActive (activeState);
	}

	private void RecordElementsLocalPositions () {
		int idx = 0;
		_elementsLocalPositions [idx++] = roundResultPanel.transform.localPosition;
		_elementsLocalPositions [idx++] = gameModeText.transform.localPosition;
		_elementsLocalPositions [idx++] = difficultyText.transform.localPosition;
		_elementsLocalPositions [idx++] = scoreSection.transform.localPosition;
		_elementsLocalPositions [idx++] = separator.transform.localPosition;
		_elementsLocalPositions [idx++] = highscoreText.transform.localPosition;
		_elementsLocalPositions [idx++] = currentScoreText.transform.localPosition;
		_elementsLocalPositions [idx++] = newHighscoreLabel.transform.localPosition;
		_elementsLocalPositions [idx++] = tapPrompt.transform.localPosition;
	}

	private void ApplyElementsLocalPositions () {
		int idx = 0;
		roundResultPanel.transform.localPosition = _elementsLocalPositions [idx++];
		gameModeText.transform.localPosition = _elementsLocalPositions [idx++];
		difficultyText.transform.localPosition = _elementsLocalPositions [idx++];
		scoreSection.transform.localPosition = _elementsLocalPositions [idx++];
		separator.transform.localPosition = _elementsLocalPositions [idx++];
		highscoreText.transform.localPosition = _elementsLocalPositions [idx++];
		currentScoreText.transform.localPosition = _elementsLocalPositions [idx++];
		newHighscoreLabel.transform.localPosition = _elementsLocalPositions [idx++];
		tapPrompt.transform.localPosition = _elementsLocalPositions [idx++];
	}
}
