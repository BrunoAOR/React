using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MenuControllerBase {

	[System.Serializable]
	public class IconSet {
		public string name;
		public float radius = 224f;
		public float angleBetweenIcons = 30f;
		public float moveDuration = 0.5f;
		public float rotateDuration = 0.5f;
		public MoveRotateUI[] iconDocks;

		[HideInInspector] public IconDockImage[] iconImages;
		private bool _isInitialized = false;

		public void Initialize () {
			SetUpIconDocks ();

			iconImages = new IconDockImage[iconDocks.Length];
			for (int i = 0; i < iconDocks.Length; i++) {
				iconImages [i] = iconDocks [i].GetComponentInChildren<IconDockImage> ();
			}

			_isInitialized = true;
		}


		private void SetUpIconDocks () {
			float zAngle = angleBetweenIcons * (iconDocks.Length - 1) / 2f;

			for (int i = 0; i < iconDocks.Length; i++) {
				iconDocks [i].targetRotation.z = zAngle;
				zAngle -= angleBetweenIcons;

				iconDocks [i].targetAnchoredPos.y = radius;

				iconDocks [i].moveDuration = this.moveDuration;
				iconDocks [i].rotateDuration = this.rotateDuration;
			}
		}


		public IEnumerator PopIconsOut (MonoBehaviour caller) {
			if (!_isInitialized)
				Initialize ();

			if (iconDocks == null || iconDocks.Length == 0)
				yield break;

			SetActive (true);

			if (iconDocks.Length > 1) {
				for (int i = 0; i < iconDocks.Length; i++) {
					caller.StartCoroutine (iconDocks [i].MoveRotate (MoveRotateUI.ActionOrder.MoveThenRotate, MoveRotateUI.ActionDirection.StartToTarget, true));
				}
			}
			yield return (caller.StartCoroutine(iconDocks [0].MoveRotate (MoveRotateUI.ActionOrder.MoveThenRotate, MoveRotateUI.ActionDirection.StartToTarget, true)));
		}


		public IEnumerator PopIconsBack (MonoBehaviour caller) {
			if (!_isInitialized)
				Initialize ();
			
			if (iconDocks == null || iconDocks.Length == 0)
				yield break;

			if (iconDocks.Length > 1) {
				for (int i = 0; i < iconDocks.Length; i++) {
					caller.StartCoroutine (iconDocks [i].MoveRotate (MoveRotateUI.ActionOrder.RotateThenMove, MoveRotateUI.ActionDirection.TargetToStart, false));
				}
			}
			yield return (caller.StartCoroutine(iconDocks [0].MoveRotate (MoveRotateUI.ActionOrder.RotateThenMove, MoveRotateUI.ActionDirection.TargetToStart, false)));

			SetActive (false);
		}


		public void SetActive (bool value) {
			if (iconDocks == null || iconDocks.Length == 0)
				return;

			iconDocks [0].transform.parent.gameObject.SetActive (value);
		}

	}

	[Header ("Data")]
	public GameModeLogic[] gameModes;
	public int[] gridSizes;
	public ButtonsBehaviour[] behaviours;

	[Header ("Icons")]
	public MultiTextGroup descriptionText;
	public IconSet[] iconSets;
	[Range (0f, 5f)]
	public float showUnlockConditionDuration = 2f;

	private bool _iconsClickable;
	private int _currentIconSetIndex;
	private int _gameModeIndex;
	private int _paceIndex;
	private int _gridSizeIndex;
	private int _behavioursIndex;
	private int[] _selectedIconNumbersPerIconSet;
	private IEnumerator _currentShowUnlockConditionCoroutine;

	[Header ("Welcome Screen")]
	public MenuIntroController introController;

	[Header ("Menu")]
	public ColorBlenderUIGraphic menuBGBlender;
	public ColorBlenderUIGraphic iconsCover;

	[Header ("Buttons")]
	public GameObject buttonsParent;
	public UnityEngine.UI.Button homeButton;
	public UnityEngine.UI.Button backButton;
	public UnityEngine.UI.Button startButton;
	public UnityEngine.UI.Button nextButton;
	public UnityEngine.UI.Button endButton;

	private Scaler _menuBGScaler;

	[Header ("Labels and Texts")]
	public MultiTextGroup instructionsLabel;
	public Text gameModeText;
	public Text paceText;
	public Text gridSizeText;
	public Text behavioursText;
	public Text highscoreLabel;
	public Text highscoreText;

	private bool _labelsWereOn  = false;

	void Awake () {
		_menuBGScaler = menuBGBlender.GetComponent <Scaler> ();

		_labelsWereOn = false;

		_gameModeIndex = -1;
		_paceIndex = -1;
		_gridSizeIndex = -1;
		_behavioursIndex = -1;
		_selectedIconNumbersPerIconSet = new int[4] {-1, -1, -1, -1};
		_currentShowUnlockConditionCoroutine = null;

		SetAllIconsActiveState (false);
		SetAllLabelsActiveState (false);
		instructionsLabel.gameObject.SetActive (false);
	}
		

	IEnumerator Start () {
		InitializeIconSets ();

		menuBGBlender.gameObject.SetActive (false);
		SetAllIconsActiveState (false);
		iconsCover.gameObject.SetActive (false);
		_iconsClickable = false;

		homeButton.gameObject.SetActive (false);
		backButton.gameObject.SetActive (false);
		startButton.gameObject.SetActive (false);
		nextButton.gameObject.SetActive (false);
		endButton.gameObject.SetActive (false);
		SetAllLabelsActiveState (false);
		instructionsLabel.gameObject.SetActive (false);

		yield return (introController.Intro ());
		StartCoroutine (PopMenu ());
	}


	private void InitializeIconSets () {
		for (int i = 0; i < iconSets.Length; i++) {
			iconSets [i].Initialize ();
		}
	}

	public override void SetActive (bool _) {
		gameObject.SetActive (_);
	}

	void OnEnable () {
		buttonsParent.SetActive (false);
		iconsCover.gameObject.SetActive (false);

		SetAllIconsActiveState (false);
		_currentIconSetIndex = 0;

		_labelsWereOn = paceText.IsActive ();
		SetAllLabelsActiveState (false);
		instructionsLabel.gameObject.SetActive (false);

		descriptionText.SetUIText ("");
		descriptionText.gameObject.SetActive (false);

	}


	private void SetAllIconsActiveState (bool value) {
		if (iconSets == null || iconSets.Length == 0)
			return;

		for (int i = 0; i < iconSets.Length; i++) {
			iconSets [i].SetActive (value);
		}
	}


	private void UpdateIconsUnlockState () {
		bool[][] iconsUnlockState = Managers.Unlockables.GetUnlockStates ();

		for (int i = 0; i < iconSets.Length; i++) {
			for (int j = 0; j < iconSets [i].iconImages.Length; j++) {
				iconSets [i].iconImages [j].UpdateUnlockState (iconsUnlockState[i][j]);
			}
		}
	}


	public override IEnumerator PopMenu () {

		// Verify and update UnlockState of icons
		UpdateIconsUnlockState ();

		_menuBGScaler.SnapToStartScale ();
		StartCoroutine (menuBGBlender.StartColorBlend ());
		yield return (_menuBGScaler.ScaleToTargetCoroutine());

		buttonsParent.SetActive (true);

		if (_labelsWereOn) {	// So this is not the first run through
			_iconsClickable = true;
			SetAllLabelsActiveState (true);
			_currentIconSetIndex = iconSets.Length;
			UpdateButtonsInteractability ();
			UpdateLabels ();
		} else {	// This is the first time the player goes through the menu (default start to Standard Mode)
			_currentIconSetIndex = 0;
			UpdateButtonsInteractability ();
			yield return (PopIconsOut ());
		}
			
	}


	private IEnumerator PopIconsOut () {
		_iconsClickable = false;
		iconsCover.gameObject.SetActive (true);
		iconsCover.UseStartColor ();

		if (iconSets!=null && iconSets.Length > 0)
			yield return (StartCoroutine (iconSets[_currentIconSetIndex].PopIconsOut (this)));

		yield return (iconsCover.StartColorBlend (true));

		instructionsLabel.gameObject.SetActive (true);
		descriptionText.gameObject.SetActive (true);
		UpdateInstructionsText (_currentIconSetIndex);
		UpdateDescriptionText (_currentIconSetIndex, _selectedIconNumbersPerIconSet [_currentIconSetIndex], true);
		UpdateLabels ();

		_iconsClickable = true;
	}


	private IEnumerator PopIconsBack () {
		_iconsClickable = false;

		iconsCover.gameObject.SetActive (true);
		iconsCover.UseStartColor ();

		instructionsLabel.gameObject.SetActive (false);
		descriptionText.gameObject.SetActive (false);
		yield return (StartCoroutine (iconSets[_currentIconSetIndex].PopIconsBack (this)));

		_iconsClickable = true;

	}


	private void UpdateInstructionsText (int iconSetIndex) {
		instructionsLabel.SelectUIText (0, iconSetIndex);
	}


	private void UpdateDescriptionText (int iconSetIndex, int iconNumber, bool iconIsUnlocked) {
		if (iconNumber == -1) {
			descriptionText.SetUIText ("");
			return;
		}

		if (iconIsUnlocked) {
			descriptionText.SelectUIText (iconSetIndex, iconNumber);
		} else {
			if (_currentShowUnlockConditionCoroutine != null) {
				StopCoroutine (_currentShowUnlockConditionCoroutine);
				_currentShowUnlockConditionCoroutine = null;
			}
			_currentShowUnlockConditionCoroutine = ShowUnlockConditionInDescriptionText (iconSetIndex, iconNumber);
			StartCoroutine (_currentShowUnlockConditionCoroutine);
		}

	}


	private IEnumerator ShowUnlockConditionInDescriptionText (int iconSetIndex, int iconNumber) {
		string message;

		message = "LOCKED!!!" + "\n";
		message += "To unlock, ";

		UnlockCondition condition = Managers.Unlockables.GetUnlockCondition (iconSetIndex, iconNumber);

		switch (condition.fieldB) {
		case UnlockCondition.FieldB.PlayCount:
			message += "play a minimum of " + condition.minValue + " times ";
			break;
		case UnlockCondition.FieldB.CumulativeScore:
			message += "reach a total of " + condition.minValue + " points ";
			break;
		}

		switch (condition.fieldA) {
		case UnlockCondition.FieldA.GameMode:
			message += "in " + condition.gameMode.ToString () + " game mode.";
			break;
		case UnlockCondition.FieldA.GamePace:
			message += "at " + condition.gamePace.ToString () + " pace.";
			break;
		case UnlockCondition.FieldA.GridSize:
			message += "in a " + condition.gridSize.ToString () + " grid size.";
			break;
		case UnlockCondition.FieldA.Behaviour:
			message += "using the " + condition.behaviour.ToString () + " modifier.";
			break;
		}

		descriptionText.SetUIText (message);
		int iconSetIndexBeforeShowingUnlockCondition = _currentIconSetIndex;
		yield return new WaitForSeconds (showUnlockConditionDuration);

		// The _currentIconSetIndex might have changed if the player clicked a move Button while the text was showing the Unlock Condition
		// If this is the case, the description text will be updated during the PopIconsOut method or should be updated (if no iconSet is opened)
		if (iconSetIndexBeforeShowingUnlockCondition == _currentIconSetIndex) {
			UpdateDescriptionText (_currentIconSetIndex, _selectedIconNumbersPerIconSet [_currentIconSetIndex], true);
		}
	}


	public void OnIconClicked (int iconNumber, bool iconIsUnlocked) {
		if (!_iconsClickable)
			return;

		UpdateDescriptionText (_currentIconSetIndex, iconNumber, iconIsUnlocked);

		if (!iconIsUnlocked) {
			Managers.Audio.PlaySFX (SFX.IconClicked_Locked);
			return;
		}

		Managers.Audio.PlaySFX (SFX.IconClicked_Unlocked);

		_selectedIconNumbersPerIconSet [_currentIconSetIndex] = iconNumber;

		// With the _selectedIconNumbersPerIconSet assigned, we can update make the nextButton.interactable = true
		nextButton.gameObject.SetActive (true);
		nextButton.interactable = true;

		switch (_currentIconSetIndex) {
		case 0:		// GameMode IconSet
			_gameModeIndex = iconNumber;
			break;
		case 1:		// Pace IconSet
			_paceIndex = iconNumber;
			break;
		case 2:		// GridSize IconSet
			_gridSizeIndex = iconNumber;
			break;
		case 3:		// Modifiers IconSet
			_behavioursIndex = iconNumber;
			break;
		}

		UpdateLabels ();

	}


	public void OnHomeClicked () {
		if (!_iconsClickable)
			return;

		Managers.Audio.PlaySFX (SFX.MenuButton);
		StartCoroutine (FirstIconSet ());
	}


	public void OnBackClicked () {
		if (!_iconsClickable)
			return;

		Managers.Audio.PlaySFX (SFX.MenuButton);
		StartCoroutine (PreviousIconSet ());
	}


	public void OnStartClicked () {
		if (!_iconsClickable)
			return;

		if (Managers.Lives.UseLife ()) {
			_iconsClickable = false;
			Managers.Audio.PlaySFX (SFX.MenuButton_GO);
			StartCoroutine (LaunchGame ());
		} else {
			Debug.Log ("No tries left!");
			RoundManager.S.PromptForAds ();
		}
	}


	public void OnNextClicked () {
		if (!_iconsClickable)
			return;

		backButton.gameObject.SetActive (true);
		if (_currentIconSetIndex == (iconSets.Length - 1)) {
			homeButton.gameObject.SetActive (true);
			endButton.gameObject.SetActive (true);
			startButton.gameObject.SetActive (true);
		}

		Managers.Audio.PlaySFX (SFX.MenuButton);
		StartCoroutine (NextIconSet ());
	}


	public void OnEndClicked () {
		if (!_iconsClickable)
			return;

		Managers.Audio.PlaySFX (SFX.MenuButton);
		StartCoroutine (LastIconSet ());
	}


	private IEnumerator FirstIconSet () {
		if (_currentIconSetIndex < iconSets.Length)
			yield return (PopIconsBack ());

		_currentIconSetIndex = 0;

		UpdateButtonsInteractability ();

		yield return (PopIconsOut ());
	}


	private IEnumerator PreviousIconSet () {
		if (_currentIconSetIndex < iconSets.Length)
			yield return (PopIconsBack ());

		_currentIconSetIndex--;

		UpdateButtonsInteractability ();

		yield return (PopIconsOut ());
	}


	private IEnumerator LaunchGame () {

		yield return new WaitForSeconds (1);
		startButton.interactable = false;
		SetAllIconsActiveState (false);
		RoundManager.S.StartGame (GetGameModeLogic(), GetGridSize(), GetButtonsBehaviours());
	}


	private IEnumerator NextIconSet () {
		yield return (PopIconsBack ());

		_currentIconSetIndex++;

		UpdateButtonsInteractability ();

		if (_currentIconSetIndex == iconSets.Length) {
			UpdateLabels ();
			yield return (iconsCover.StartColorBlend (true));
		} else {
			yield return (PopIconsOut ());
			UpdateLabels ();
		}
	}


	private IEnumerator LastIconSet () {
		yield return (PopIconsBack ());

		_currentIconSetIndex = iconSets.Length;

		UpdateButtonsInteractability ();

		UpdateLabels ();
		yield return (iconsCover.StartColorBlend (true));
	}


	private void SetAllLabelsActiveState (bool value) {
		gameModeText.gameObject.SetActive (value);
		paceText.gameObject.SetActive (value);
		gridSizeText.gameObject.SetActive (value);
		behavioursText.gameObject.SetActive (value);
		highscoreLabel.gameObject.SetActive (value);
		highscoreText.gameObject.SetActive (value);
	}


	private void UpdateLabels () {
		string tText = "";

		switch (_currentIconSetIndex) {
		case 0:		// GameMode Selection
			gameModeText.gameObject.SetActive (true);
			switch (_gameModeIndex) {
			case 0:
				tText = "Standard";
				break;
			case 1:
				tText = "Inverse";
				break;
			case 2:
				tText = "Target";
				break;
			}
			gameModeText.text = tText;
			break;
		
		case 1:		// Pace Selection
			paceText.gameObject.SetActive (true);
			switch (_paceIndex) {
			case 0:
				tText = "Relaxed";
				break;
			case 1:
				tText = "Hectic";
				break;
			}
			paceText.text = tText;
			break;

		case 2:		// GridSize Selection
			gridSizeText.gameObject.SetActive (true);
			switch (_gridSizeIndex) {
			case 0:
				tText = "Small";
				break;
			case 1:
				tText = "Medium";
				break;
			case 2:
				tText = "Large";
				break;
			}
			gridSizeText.text = tText;
			break;

		case 3:		// Behaviours Selection
			behavioursText.gameObject.SetActive (true);
			switch (_behavioursIndex) {
			case 0:
				tText = "None";
				break;
			case 1:
				tText = "Ghost";
				break;
			case 2:
				tText = "Motion";
				break;
			case 3:
				tText = "Ghost+Motion";
				break;
			}
			behavioursText.text = tText;
			break;

		case 4:		// Time to Start game (Highscores can be shown)
			highscoreLabel.gameObject.SetActive (true);
			highscoreText.gameObject.SetActive (true);
			break;
		}

		// Highscore is always updated (if the selection indexes have been changed from the default -1), so that (if showing) it will update accoding to the selection.
		if (_gameModeIndex != -1 && _paceIndex != -1 && _gridSizeIndex != -1 && _behavioursIndex != -1) {
			highscoreText.text = (GetHighscore ()).ToString ();
		}

	}


	private void UpdateButtonsInteractability () {

		if (_currentIconSetIndex == iconSets.Length) {
			nextButton.interactable = false;
			endButton.interactable = false;
		} else {
			if (_selectedIconNumbersPerIconSet [_currentIconSetIndex] == -1) {
				nextButton.interactable = false;
				endButton.interactable = false;
			} else {
				nextButton.interactable = true;
				endButton.interactable = true;
			}
		}

		if (_currentIconSetIndex == 0) {
			homeButton.interactable = false;
			backButton.interactable = false;
		} else {
			homeButton.interactable = true;
			backButton.interactable = true;
		}

		bool shouldMakeStartButtonInteractable = true;
		for (int i = 0; i < _selectedIconNumbersPerIconSet.Length; i++) {
			shouldMakeStartButtonInteractable &= (_selectedIconNumbersPerIconSet[i] != -1);
		}
		if (shouldMakeStartButtonInteractable) {
			startButton.interactable = true;
		} else {
			startButton.interactable = false;
		}


	}


	public bool IconsInteractable () {
		return _iconsClickable;
	}


	private GameModeLogic GetGameModeLogic () {
		return gameModes[_gameModeIndex * 2 + _paceIndex];
	}


	private int GetGridSize () {
		int gridSize = 3;
		switch (_gridSizeIndex) {	// Case 0 is the default value
		case 1:
			gridSize = 4;
			break;
		case 2:
			gridSize = 5;
			break;
		}

		return gridSize;
	}


	private ButtonsBehaviour[] GetButtonsBehaviours () {
		int arraySize = 0;
		ButtonsBehaviour[] buttonsBehaviours =  new ButtonsBehaviour[arraySize];

		switch (_behavioursIndex) {	// Case 0 is the default: arraySize: 0, empty array
		case 1:
			arraySize = 1;
			buttonsBehaviours = new ButtonsBehaviour[] { behaviours [0] };
			break;
		case 2:
			arraySize = 1;
			buttonsBehaviours = new ButtonsBehaviour[] { behaviours [1] };
			break;
		case 3:
			arraySize = 2;
			buttonsBehaviours = behaviours;
			break;
		}

		return buttonsBehaviours;
	}


	private int GetHighscore () {
		return (Managers.Score.GetHighscore ( GetGameModeLogic(), GetGridSize (), GetButtonsBehaviours() ));
	}
		
}