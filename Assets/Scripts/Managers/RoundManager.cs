using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameStage {
	preStart,
	Timed,
	GameEnd
}

[RequireComponent (typeof (BoardManager))]
public class RoundManager : MonoBehaviour {

	public static RoundManager S;

	[Header ("GAME MODE LOGIC")]
	public GameModeLogic modeLogic;
	public ButtonsBehaviour[] modeBehaviours;

	[Header ("Game info")]
	public GameMode gameMode;
	public Difficulty difficulty;

	[Header ("For Target Mode")]
	public GameObject targetModeSection;
	public Image targetImage;

	[Header ("Menu and Background Reference")]
	public MenuControllerBase menuController;
	public BGLineSpawner background;
	[Range (0f,1f)]
	public float alphaAtMenu = 1.0f;
	[Range (0f,1f)]
	public float alphaAtGame = 0.1f;
	[Range (0f,1f)]
	public float alphaAtEndGame = 0.3f;
	public GameObject menuTapPrompt;

	[Header ("Ads")]
	public AdsController AdsController;

	[Header ("Game Stage")]
	public GameObject HUDSection;
	public GameStage gameStage;

	[Header ("Game Description")]
	public GameObject descriptionSubSection;
	public Text descriptionTitleText;
	public Text descriptionText;
	public FlashingUIText descriptionTapPrompt;
	public AudioSource _audioSource;
	public AudioClip typingSound;
	private bool _typingDescription = false;

	[Header ("Initial Count Down")]
	public CountDownUIText countDown;
	public ColorBlenderUIGraphic coverImage;

	[Header ("Game Timer")]
	public AnimatedUIText animatedTimerText;
	private float _timer;

	[Header ("For the buttons")]
	public int buttonsOn;
	private Button[] _buttons;

	[Header ("Game Pause Penalty")]
	public float gamePausePenalty = -0.3f;
	public PauseMenu pauseMenuController;
	public RectTransform pauseButtonRectTransform;

	private float _lastUnpauseStartTime;
	private float _lastUnpauseDuration = 0f;

	[Header ("End Game")]
	public Text endGameText;
	public GameObject endGameTapPrompt;

	[HideInInspector] public BoardManager boardManager;
	private IEnumerator _currentGameLoop;
	private IEnumerator _currentTimedStage;
	private bool _isRunningGameLoop;
	private bool _isPaused;
	private bool _buttonsClickable;
	private string _score;
	private string _highscoreString;
	private string _highscoreBeatenString {
		get {
			return (string.Format ("You beat the highscore of {0} with {1} points!", _highscoreString, _score));
		}
	}
	private string _highscoreNotBeatenString {
		get {
			return (string.Format ("{0} points were not enough to beat the highscore of {1}...", _score, _highscoreString));
		}
	}


	void Awake () {
		if (S == null) {
			S = this;
		} else if (S != this) {
			Destroy (gameObject);
		}

		boardManager = GetComponent<BoardManager> ();

		// Turn off all UICanvas sections
		background.gameObject.SetActive (false);
		targetModeSection.SetActive (false);
		menuController.SetActive (false);
		HUDSection.SetActive (false);
		pauseMenuController.gameObject.SetActive (false);
		AdsController.gameObject.SetActive (false);
	}


	void Start () {

		Managers.Audio.PlayMusic1 ();

		background.gameObject.SetActive (true);
		background.SetAlpha (alphaAtMenu);

		// Control is passed on to MenuController.cs (runs Start method)
		menuController.SetActive (true);
	}

	public void StartGame (GameMode gameMode, Difficulty difficulty) {
		this.gameMode = gameMode;
		this.difficulty = difficulty;
		GameModeLogic logic = Managers.Enums.GetGameModeLogic (gameMode);
		GameDifficulty gameDifficulty = Managers.Enums.GetGameDifficulty (difficulty);
		StartGame (logic, (int)gameDifficulty.gridSize, gameDifficulty.buttonsBehaviours);
	}

	public void StartGame (GameModeLogic selectedModeLogic, int gridSize, ButtonsBehaviour[] selectedButtonBehaviours) {
		modeLogic = selectedModeLogic;
		modeLogic.InitializeGameMode ();
		boardManager.gridSize = gridSize;
		modeBehaviours = selectedButtonBehaviours;

		menuController.SetActive (false);
		endGameText.gameObject.SetActive (false);
		endGameTapPrompt.SetActive (false);

		// Description related
		descriptionSubSection.SetActive (false);
		descriptionText.gameObject.SetActive (true);
		descriptionTitleText.gameObject.SetActive (true);
		descriptionTapPrompt.gameObject.SetActive (true);

		HUDSection.SetActive (true);
		_timer = modeLogic.startTime;
		animatedTimerText.AdjustFreeTimeScale (modeLogic.startTime, 0f);
		animatedTimerText.ApplyStartConditions ();
		animatedTimerText.gameObject.SetActive (true);
		SetTimerText ();
		boardManager.SetUpGrid ();

		_buttons = boardManager.GetButtons ();
		_buttonsClickable = false;

		for (int i = 0; i < modeBehaviours.Length; i++) {
			modeBehaviours [i].InitializeBehaviour (_buttons);
		}

		_currentGameLoop = GameLoop ();
		StartCoroutine (_currentGameLoop);
	}


	void Update () {
		if (_isRunningGameLoop && !_isPaused) {
			if (gameStage == GameStage.Timed) {
				for (int i = 0; i < modeBehaviours.Length; i++) {
					modeBehaviours [i].RunBehaviour (_lastUnpauseDuration);
				}
				if (_lastUnpauseDuration != 0)
					_lastUnpauseDuration = 0;
			}

			if (buttonsOn > 0) {
				_timer -= Time.deltaTime;
				if (_timer <= 0) {
					_timer = 0;
					TurnOffButtons ();
				}
				SetTimerText ();
			}
		}
	}


	public void RushTyping () {
		if (_typingDescription) {
			Managers.Audio.PlaySFX (SFX.TapPrompt);
			TextTyper.RushTyping ();
		}
	}


	private IEnumerator GameLoop () {
		_isRunningGameLoop = true;
		background.SetAlpha (alphaAtGame);

		yield return (preStartStage ());

		_currentTimedStage = (TimedGame ());
		yield return (_currentTimedStage);
		_currentTimedStage = null;
		background.SetAlpha (alphaAtEndGame);
		yield return (GameEnd ());
		_currentGameLoop = null;
		_isRunningGameLoop = false;
	}


	private IEnumerator preStartStage () {
		Debug.Log ("Pre Start");
		gameStage = GameStage.preStart;
		Managers.Score.ResetScore ();

		coverImage.gameObject.SetActive (true);
		coverImage.UseStartColor ();

		descriptionSubSection.gameObject.SetActive (true);
		descriptionText.text = "";
		descriptionTitleText.text = modeLogic.modeName;

		descriptionTapPrompt.SetText ("Tap anywhere to speed up...");

		yield return new WaitForSeconds (0.5f);

		_typingDescription = true;
		yield return ( StartCoroutine (TextTyper.TypeText(this, descriptionText, modeLogic.modeDescription, _audioSource, typingSound)) );
		_typingDescription = false;

		descriptionTapPrompt.SetText ("Tap anywhere to continue...");

		while (!Input.GetMouseButtonDown (0)) {
			yield return null;
		}

		Managers.Audio.PlaySFX (SFX.TapPrompt);
		descriptionSubSection.SetActive (false);

		Managers.Audio.PlayMusic2 ();

		yield return new WaitForSeconds (0.25f);

		yield return (StartCoroutine (countDown.StartCountDown ()) );

		pauseMenuController.gameObject.SetActive (true);
		yield return (StartCoroutine (coverImage.StartColorBlend (true)) );
	}


	private IEnumerator TimedGame () {
		Debug.Log ("Timed stage");
		gameStage = GameStage.Timed;

		bool wasPaused = false;

		if (modeLogic.GetType() == typeof(TargetMode)) {
			targetModeSection.SetActive (true);
			targetImage.gameObject.SetActive (false);
			TargetMode targetMode = modeLogic as TargetMode;
			targetMode.targetImage = targetImage;
		}

		while (_timer > 0) {

			float waitTime = Random.Range (modeLogic.minWaitTime, modeLogic.maxWaitTime);
			yield return new WaitForSeconds (waitTime);

			while (_isPaused) {
				wasPaused = true;
				yield return null;
			}

			if (wasPaused) {
				yield return new WaitForSeconds (waitTime);
				wasPaused = false;
			}

			TurnOnButtons ();
			_buttonsClickable = true;

			while (buttonsOn > 0) {
				yield return null;
			}
			_buttonsClickable = false;
		}
	}


	private IEnumerator GameEnd () {
		Debug.Log ("GAME ENDED");
		gameStage = GameStage.GameEnd;
		targetModeSection.SetActive (false);

		for (int i = 0; i < modeBehaviours.Length; i++) {
			modeBehaviours [i].StopBehaviour ();
		}

		Managers.Audio.PlayMusic1 ();

		// Turn off the pauseMenuCanvas
		pauseMenuController.gameObject.SetActive (false);

		// Wait
		yield return new WaitForSeconds (0.5f);

		// Move all buttons if enabled
		if (_buttons [0].GetComponent<Mover> () != null) {	// If one button has a Mover, then all of them should have it
			Mover bMover;
			for (int i = 1; i < _buttons.Length; i++) {
				bMover = _buttons [i].GetComponent<Mover> ();
				if (bMover != null) {
					StartCoroutine (bMover.MoveToTargetCoroutine (true));
				}
			}
			bMover = _buttons [0].GetComponent<Mover> ();
			if (bMover != null) {
				yield return (bMover.MoveToTargetCoroutine (true));
			}
		}
		// End of buttons moving

		// Wait again
		yield return new WaitForSeconds (0.5f);

		// Remove all buttons and timer
		_buttonsClickable = false;
		boardManager.ClearGrid ();
		animatedTimerText.gameObject.SetActive (false);

		// Turn off the TargetModeCanvas
		targetModeSection.SetActive (false);

		// Save highscores if achieved and show a message about the score reached and if highscore was beaten or not.
		endGameText.gameObject.SetActive (true);
		_score = Managers.Score.GetScore ().ToString ();
		_highscoreString = Managers.Score.GetHighscore (gameMode, difficulty).ToString ();

		bool newHighScore = Managers.Score.SetHighscore (gameMode, difficulty);
		if (newHighScore) {
			Debug.Log ("Score: " + Managers.Score.GetScore() + "|Old High Score of " + _highscoreString + " beaten.");
			endGameText.text = _highscoreBeatenString;
		} else {
			endGameText.text = _highscoreNotBeatenString;
			Debug.Log ("Score: " + Managers.Score.GetScore() + "|High Score of " + _highscoreString + " remains undefeated.");
		}

		// Register the round in the StatsManager
		Managers.Stats.RegisterPlay (gameMode, difficulty, Managers.Score.GetScore());

		endGameTapPrompt.SetActive (true);
		// Wait for a tap
		while (!Input.GetMouseButtonDown (0)) {
			yield return null;
		}

		Managers.Audio.PlaySFX (SFX.TapPrompt);
		endGameText.gameObject.SetActive (false);
		endGameTapPrompt.SetActive (false);

		// Turn off the HUD (Timer, score, highscore)
		HUDSection.SetActive (false);

		// Set background alpha
		background.SetAlpha (alphaAtMenu);

		// Control is passed on to MenuController.cs
		menuController.SetActive (true);
		StartCoroutine (menuController.PopMenu () );

	}


	public void QuitRound () {
		_isPaused = false;

		// Ensure buttonsOn = 0
		buttonsOn = 0;

		// Stop TimedGame coroutine
		StopCoroutine (_currentTimedStage);
		_currentTimedStage = null;

		// Stop the Game Loop
		StopCoroutine (_currentGameLoop);
		_currentGameLoop = null;
		_isRunningGameLoop = false;

		// Turn off the pauseMenuCanvas
		pauseMenuController.gameObject.SetActive (false);

		// Remove all buttons and timer
		boardManager.ClearGrid ();
		animatedTimerText.gameObject.SetActive (false);

		// Turn off the TargetModeCanvas
		targetModeSection.SetActive (false);

		// Turn off the HUD (Timer, score, highscore)
		HUDSection.SetActive (false);

		// Change music
		Managers.Audio.PlayMusic1 ();

		// Set background alpha
		background.SetAlpha (alphaAtMenu);

		// Control is passed on to MenuController.cs
		menuController.SetActive (true);
		StartCoroutine (menuController.PopMenu () );

	}


	private GameMode GetStatsGameMode () {
		return modeLogic.gameMode;
	}

	private GamePace GetStatsGamePace () {
		return modeLogic.gamePace;
	}

	private GridSize GetStatsGridSize () {
		return ((GridSize)boardManager.gridSize);
	}

	private Behaviour GetStatsBehaviour () {
		if (modeBehaviours.Length == 0) {
			return Behaviour.None;
		}

		if (modeBehaviours.Length == 2) {
			return Behaviour.GhostMotion;
		}

		return modeBehaviours [0].statsBehaviour;
	}

	public void PromptForAds () {
		AdsController.gameObject.SetActive (true);
	}


	public void Pause () {
		_isPaused = true;

		if (gameStage == GameStage.Timed) {
			_timer += gamePausePenalty;
			SetTimerText ();

			Vector2 pausePenaltyPosition = new Vector2 (0.75f, (Camera.main.ScreenToViewportPoint (pauseButtonRectTransform.position)).y);
			PointsSpawner.S.AnimateBadTimeBonus (Utils.GetSignedStringFromNumber (gamePausePenalty), pausePenaltyPosition);
		}
	}


	public void UnpausingStarted () {
		if (gameStage == GameStage.Timed) {
			_lastUnpauseStartTime = Time.time;
		} else {
			_lastUnpauseStartTime = -1;
		}
	}


	public void Unpause () {
		if (gameStage == GameStage.Timed && _lastUnpauseStartTime != -1) {
			_lastUnpauseDuration = Time.time - _lastUnpauseStartTime;
		}
		_isPaused = false;
	}


	public void ButtonPressed (Button button) {
		if (!_buttonsClickable)
			return;


		float timeBonus;

		int changeInButtonsOnAmount = modeLogic.ButtonPressed (button, out timeBonus);
		if (changeInButtonsOnAmount != 0) {
			if (modeLogic.minWaitTime >= 0.25f)
				Managers.Audio.PlaySFX (SFX.ButtonUnlit);

			buttonsOn += changeInButtonsOnAmount;
		}

		_timer += timeBonus;
		SetTimerText ();
	}


	private void TurnOnButtons () {
		buttonsOn += modeLogic.TurnOnButtons (_buttons);
		Managers.Audio.PlaySFX (SFX.ButtonsOn);
	}


	public void TurnOffButtons () {
		buttonsOn += modeLogic.TurnOffButtons (_buttons);
	}


	private void SetTimerText () {
		animatedTimerText.SetTime (_timer);
	}


	private void OnValidate () {
		if (gamePausePenalty > 0) {
			gamePausePenalty = 0;
		}
	}

}
