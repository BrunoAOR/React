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
	public GameDifficulty gameDifficulty;
	public ButtonsBehaviour[] modeBehaviours;

	[Header ("Game info")]
	public GameMode gameMode;
	public Difficulty difficulty;

	private int currentRound;
	private float roundWaitTime;

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

	[Header ("Ads")]
	public AdsController adsController;

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
	public int buttonsLeftToClick;
	private Button[] _buttons;

	[Header ("Game Pause Penalty")]
	public float gamePausePenalty = -0.3f;
	public PauseMenu pauseMenuController;
	public RectTransform pauseButtonRectTransform;

	private float _lastUnpauseStartTime;
	private float _lastUnpauseDuration = 0f;

	[Header ("End Game")]
	public RoundResultController roundResultController;

	[HideInInspector] public BoardManager boardManager;
	private IEnumerator _currentGameLoop;
	private IEnumerator _currentTimedStage;
	private bool _isRunningGameLoop;
	private bool _isPaused;
	private bool _buttonsClickable;

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
		roundResultController.SetActive (false);
		pauseMenuController.gameObject.SetActive (false);
		adsController.gameObject.SetActive (false);

		// Set up some references
		menuController.SetAdsController(adsController);
		adsController.menuController = menuController;
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
		gameDifficulty = Managers.Enums.GetGameDifficulty (difficulty);
		StartGame (logic, (int)gameDifficulty.gridSize, gameDifficulty.buttonsBehaviours);
	}

	public void StartGame (GameModeLogic selectedModeLogic, int gridSize, ButtonsBehaviour[] selectedButtonBehaviours) {
		modeLogic = selectedModeLogic;
		modeLogic.InitializeGameMode ();
		boardManager.gridSize = gridSize;
		modeBehaviours = selectedButtonBehaviours;

		currentRound = 0;
		roundWaitTime = 0;

		menuController.SetActive (false);

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

			if (buttonsLeftToClick > 0) {
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

		// Actual round loop
		WaitForSeconds wfs;


		while (_timer > 0) {
			// Increase currentRound and get waitTime for the round.
			currentRound++;
			roundWaitTime = gameDifficulty.GetWaitTime (currentRound);
			wfs = new WaitForSeconds (roundWaitTime);

			// Wait before turning buttons on
			yield return wfs;

			// Controlling pausing the game
			// The outer while-loop makes sure that pausing 'after unpausing but before wfs goes through' is still caught.
			while (_isPaused || wasPaused) {
				// If the game is paused, just wait before taking action
				while (_isPaused) {
					wasPaused = true;
					yield return null;
				}

				// When unpausing, repeat the full waitTime;
				if (wasPaused) {
					wasPaused = false;
					yield return wfs;
				}
			}

			// Turn on buttons and start responding to clicks on buttons
			TurnOnButtons ();
			_buttonsClickable = true;

			// Wait around while a button is still on.
			while (buttonsLeftToClick > 0) {
				yield return null;
			}

			// Stop responding to button clicks after the round has completed
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

		// Turn off the TargetModeCanvas
		targetModeSection.SetActive (false);

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

		// Turn off the HUDSection (Timer, score, etc.)
		HUDSection.SetActive (false);

		// Set background alpha
		background.SetAlpha (alphaAtMenu);

		// Force garbage collection
		System.GC.Collect ();

		// Control is passed on to the RoundResultController.
		roundResultController.ShowRoundResult (gameMode, difficulty, Managers.Score.GetScore ());
	}


	public void QuitRound () {
		_isPaused = false;

		// Ensure buttonsOn = 0
		buttonsLeftToClick = 0;

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

		// Go to menu
		GoToMenu ();
	}
		

	public void GoToMenu () {
		// Set background alpha
		background.SetAlpha (alphaAtMenu);

		// Control is passed on to the MenuController.
		menuController.SetActive (true);
		StartCoroutine (menuController.PopMenu () );
	}


	public void Pause () {
		_isPaused = true;

		if (gameStage == GameStage.Timed) {
			_timer += gamePausePenalty;
			if (_timer < 0) {
				_timer = 0;
			}
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
			if (roundWaitTime >= 0.25f) {
				Managers.Audio.PlaySFX (SFX.ButtonUnlit);
			}
			buttonsLeftToClick += changeInButtonsOnAmount;
		}

		_timer += timeBonus;
		if (_timer < 0) {
			_timer = 0;
		}
		SetTimerText ();
	}


	private void TurnOnButtons () {
		buttonsLeftToClick += modeLogic.TurnOnButtons (_buttons, gameDifficulty.GetButtonsToClick(currentRound));
		Managers.Audio.PlaySFX (SFX.ButtonsOn);
	}


	public void TurnOffButtons () {
		buttonsLeftToClick += modeLogic.TurnOffButtons (_buttons);
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
