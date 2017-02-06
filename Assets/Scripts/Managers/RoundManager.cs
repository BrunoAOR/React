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
	
	[Header ("Game info")]
	[ShowOnly] public GameMode gameMode;
	[ShowOnly] public Difficulty difficulty;

	private GameModeLogic _modeLogic;
	private GameDifficulty _gameDifficulty;
	private ButtonsBehaviour[] _modeBehaviours;
	private int _currentRound;
	private float _roundWaitTime;

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
	[ShowOnly] public GameStage gameStage;

	[Header ("Game Description")]
	public GameObject descriptionSubSection;
	public LanguageText descriptionTitleLangtText;
	public LanguageText descriptionLangText;
	public LanguageText descriptionLangTextTapPrompt;
	public string continueTapPropmtKeyword = "continueTapPrompt";
	public string speedUpTapPromptKeyword = "speedUpTapPrompt";
	public AudioSource _audioSource;
	public AudioClip typingSound;

	private TextTyper _textTyper;
	private bool _typingDescription = false;

	[Header ("Initial Count Down")]
	public CountDownUIText countDown;
	public ColorBlenderUIGraphic coverImage;

	[Header ("Game Timer")]
	public AnimatedUIText animatedTimerText;

	private float _timer;

	[Header ("For the buttons")]
	[ShowOnly] [SerializeField] private int _buttonsLeftToClick;
	private CircleButton[] _buttons;

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
		_textTyper = new TextTyper ();

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
		_gameDifficulty = Managers.Enums.GetGameDifficulty (difficulty);
		StartGame (logic, (int)_gameDifficulty.gridSize, _gameDifficulty.buttonsBehaviours);
	}

	public void StartGame (GameModeLogic selectedModeLogic, int gridSize, ButtonsBehaviour[] selectedButtonBehaviours) {
		_modeLogic = selectedModeLogic;
		_modeLogic.InitializeGameMode ();
		boardManager.gridSize = gridSize;
		_modeBehaviours = selectedButtonBehaviours;

		_currentRound = 0;
		_roundWaitTime = 0;

		menuController.SetActive (false);

		// Description related
		descriptionSubSection.SetActive (false);
		descriptionLangText.gameObject.SetActive (true);
		descriptionTitleLangtText.gameObject.SetActive (true);
		descriptionLangTextTapPrompt.gameObject.SetActive (true);

		HUDSection.SetActive (true);
		_timer = _modeLogic.startTime;
		animatedTimerText.AdjustFreeTimeScale (_modeLogic.startTime, 0f);
		animatedTimerText.ApplyStartConditions ();
		animatedTimerText.gameObject.SetActive (true);
		SetTimerText ();
		boardManager.SetUpGrid ();

		_buttons = boardManager.GetButtons ();
		_buttonsClickable = false;

		for (int i = 0; i < _modeBehaviours.Length; i++) {
			_modeBehaviours [i].InitializeBehaviour (_buttons);
		}

		// Force garbage collection before starting the game
		System.GC.Collect ();

		_currentGameLoop = GameLoop ();
		StartCoroutine (_currentGameLoop);
	}


	void Update () {
		if (_isRunningGameLoop && !_isPaused) {
			if (gameStage == GameStage.Timed) {
				for (int i = 0; i < _modeBehaviours.Length; i++) {
					_modeBehaviours [i].RunBehaviour (_lastUnpauseDuration);
				}
				if (_lastUnpauseDuration != 0)
					_lastUnpauseDuration = 0;
			}

			if (_buttonsLeftToClick > 0) {
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
			_textTyper.RushTyping ();
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
		Debug.Log ("PRE START");
		gameStage = GameStage.preStart;
		Managers.Score.ResetScore ();

		coverImage.gameObject.SetActive (true);
		coverImage.UseStartColor ();

		descriptionSubSection.gameObject.SetActive (true);
		descriptionLangText.SetText ("");
		descriptionTitleLangtText.ApplyTranslation (gameMode.ToString ());

		descriptionLangTextTapPrompt.ApplyTranslation (speedUpTapPromptKeyword);

		Managers.Audio.StopMusic ();
		yield return new WaitForSeconds (0.5f);

		_typingDescription = true;

		string textToType = descriptionLangText.GetTranslation (gameMode.ToString() + "Description");
		yield return ( StartCoroutine (_textTyper.TypeText(this, descriptionLangText.GetConnectedText(), textToType, _audioSource, typingSound)) );
		_typingDescription = false;

		descriptionLangTextTapPrompt.ApplyTranslation (continueTapPropmtKeyword);

		while (!Input.GetMouseButtonDown (0)) {
			yield return null;
		}

		Managers.Audio.PlaySFX (SFX.TapPrompt);
		descriptionSubSection.SetActive (false);

		yield return new WaitForSeconds (0.25f);

		yield return (StartCoroutine (countDown.StartCountDown ()) );

		Managers.Audio.PlayMusic2 ();

		yield return (StartCoroutine (coverImage.StartColorBlend (true)) );
		pauseMenuController.gameObject.SetActive (true);
	}


	private IEnumerator TimedGame () {
		Debug.Log ("TIMED STAGE");
		gameStage = GameStage.Timed;

		if (_modeLogic.GetType() == typeof(TargetMode)) {
			targetModeSection.SetActive (true);
			targetImage.gameObject.SetActive (false);
			TargetMode targetMode = _modeLogic as TargetMode;
			targetMode.targetImage = targetImage;
		}

		// Actual round loop
		float waitTimeRemaining;

		while (_timer > 0) {
			// Increase currentRound and get waitTime for the round.
			_currentRound++;
			_roundWaitTime = _gameDifficulty.GetWaitTime (_currentRound);
			waitTimeRemaining = _roundWaitTime;

			while (waitTimeRemaining > 0) {
				if (!_isPaused) {
					waitTimeRemaining -= Time.deltaTime;
				}
				yield return null;
			}

			// Turn on buttons and start responding to clicks on buttons
			TurnOnButtons ();
			_buttonsClickable = true;

			// Wait around while a button is still on.
			while (_buttonsLeftToClick > 0) {
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

		for (int i = 0; i < _modeBehaviours.Length; i++) {
			_modeBehaviours [i].StopBehaviour ();
		}

		// Turn off the pauseMenuCanvas
		pauseMenuController.gameObject.SetActive (false);

		// Turn off the TargetModeCanvas
		targetModeSection.SetActive (false);

		Managers.Audio.StopMusic ();

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

		// Change music
		Managers.Audio.PlayMusic1 ();

		// Control is passed on to the RoundResultController.
		roundResultController.ShowRoundResult (gameMode, difficulty, Managers.Score.GetScore ());
	}


	public void QuitRound () {
		_isPaused = false;

		// Ensure buttonsOn = 0
		_buttonsLeftToClick = 0;

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


	public void ButtonPressed (CircleButton button) {
		if (!_buttonsClickable)
			return;


		float timeBonus;

		int changeInButtonsOnAmount = _modeLogic.ButtonPressed (button, out timeBonus);
		if (changeInButtonsOnAmount != 0) {
			if (_roundWaitTime >= 0.25f) {
				Managers.Audio.PlaySFX (SFX.ButtonUnlit);
			}
			_buttonsLeftToClick += changeInButtonsOnAmount;
		}

		_timer += timeBonus;
		if (_timer < 0) {
			_timer = 0;
		}
		SetTimerText ();
	}


	private void TurnOnButtons () {
		_buttonsLeftToClick += _modeLogic.TurnOnButtons (_buttons, _gameDifficulty.GetButtonsToClick(_currentRound));
		Managers.Audio.PlaySFX (SFX.ButtonsOn);
	}


	public void TurnOffButtons () {
		_buttonsLeftToClick += _modeLogic.TurnOffButtons (_buttons);
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
