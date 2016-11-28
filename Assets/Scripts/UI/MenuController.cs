using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	[System.Serializable]
	public class IconSet {
		public string name;
		public float radius = 224f;
		public float angleBetweenIcons = 30f;
		public float moveDuration = 0.5f;
		public float rotateDuration = 0.5f;
		public MoveRotateUI[] iconDocks;

		private bool _isInitialized = false;

		void Initialize () {
			SetUpIconDocks ();
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
	public IconSet[] iconSets;

	private bool _iconsClickable;
	private int _currentIconSetIndex;
	private int _gameModeIndex;
	private int _paceIndex;
	private int _gridSizeIndex;
	private int _behavioursIndex;

	[Header ("Welcome Screen")]
	public UIMover logoMover;
	public Text welcomeText;
	public GameObject tapPrompt;

	private ColorBlenderUIGraphic _welcomeTextBlender;

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
	public Text gameModeLabel;
	public Text gameModeText;
	public Text paceLabel;
	public Text paceText;
	public Text gridSizeLabel;
	public Text gridSizeText;
	public Text behavioursLabel;
	public Text behavioursText;
	public Text highscoreLabel;
	public Text highscoreText;

	private bool _labelsWereOn  = false;

	void Awake () {
		_welcomeTextBlender = welcomeText.GetComponent<ColorBlenderUIGraphic> ();
		_menuBGScaler = menuBGBlender.GetComponent <Scaler> ();

		_labelsWereOn = false;

		_gameModeIndex = 0;
		_paceIndex = 0;
		_gridSizeIndex = 0;
		_behavioursIndex = 0;

		SetAllIconsActiveState (false);
		SetAllLabelsActiveState (false);
	}
		

	IEnumerator Start () {
		logoMover.SnapToStart ();
		welcomeText.gameObject.SetActive (true);
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

		tapPrompt.SetActive (true);

		while (!Input.GetMouseButtonDown (0))
			yield return null;

		tapPrompt.SetActive (false);


		StartCoroutine (LogoToMenu() );
	}


	private IEnumerator LogoToMenu () {
		welcomeText.gameObject.SetActive (true);

		// Blend out and setActive(false) for the welcomeText
		StartCoroutine (_welcomeTextBlender.StartColorBlend () );

		// Dim out and then back to full alpha for the logo...
		StartCoroutine (logoMover.GetComponent<ColorBlenderUIGraphic> ().StartColorBlend (false));
		// ... as it moves up
		yield return (logoMover.MoveToTarget ());

		yield return (PopMenu ());

	}


	void OnEnable () {
		buttonsParent.SetActive (false);
		iconsCover.gameObject.SetActive (false);

		SetAllIconsActiveState (false);
		_currentIconSetIndex = 0;

		_labelsWereOn = paceText.IsActive ();
		SetAllLabelsActiveState (false);

	}


	private void SetAllIconsActiveState (bool value) {
		if (iconSets == null || iconSets.Length == 0)
			return;

		for (int i = 0; i < iconSets.Length; i++) {
			iconSets [i].SetActive (value);
		}
	}


	public IEnumerator PopMenu () {

		_menuBGScaler.SnapToStartScale ();
		StartCoroutine (menuBGBlender.StartColorBlend ());
		yield return (_menuBGScaler.ScaleToTargetCoroutine());

		_iconsClickable = true;
		buttonsParent.SetActive (true);
		nextButton.gameObject.SetActive (true);

		if (_labelsWereOn) {
			SetAllLabelsActiveState (true);
			_currentIconSetIndex = iconSets.Length;
			homeButton.interactable = true;
			backButton.interactable = true;
			startButton.interactable = true;
			nextButton.interactable = false;
			endButton.interactable = false;
		} else {
			_currentIconSetIndex = 0;
			homeButton.interactable = false;
			backButton.interactable = false;
			startButton.interactable = true;
			nextButton.interactable = true;
			endButton.interactable = true;
			StartCoroutine (PopIconsOut ());
		}
			
	}


	private IEnumerator PopIconsOut () {
		_iconsClickable = false;
		iconsCover.gameObject.SetActive (true);
		iconsCover.UseStartColor ();

		if (iconSets!=null && iconSets.Length > 0)
			yield return (StartCoroutine (iconSets[_currentIconSetIndex].PopIconsOut (this)));

		yield return (iconsCover.StartColorBlend (true));

		UpdateLabels ();

		_iconsClickable = true;
	}


	private IEnumerator PopIconsBack () {
		_iconsClickable = false;

		iconsCover.gameObject.SetActive (true);
		iconsCover.UseStartColor ();

		yield return (StartCoroutine (iconSets[_currentIconSetIndex].PopIconsBack (this)));

		_iconsClickable = true;

	}

	public void OnIconClicked (int iconNumber) {
		if (!_iconsClickable)
			return;

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

		StartCoroutine (FirstIconSet ());
	}


	public void OnBackClicked () {
		if (!_iconsClickable)
			return;

		StartCoroutine (PreviousIconSet ());
	}


	public void OnStartClicked () {
		if (!_iconsClickable)
			return;

		if (Managers.Lives.UseLife ()) {
			_iconsClickable = false;
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

		StartCoroutine (NextIconSet ());
	}


	public void OnEndClicked () {
		if (!_iconsClickable)
			return;

		StartCoroutine (LastIconSet ());
	}


	private IEnumerator FirstIconSet () {
		if (_currentIconSetIndex < iconSets.Length)
			yield return (PopIconsBack ());

		_currentIconSetIndex = 0;

		homeButton.interactable = false;
		backButton.interactable = false;
		nextButton.interactable = true;
		endButton.interactable = true;

		yield return (PopIconsOut ());
	}


	private IEnumerator PreviousIconSet () {
		if (_currentIconSetIndex < iconSets.Length)
			yield return (PopIconsBack ());

		_currentIconSetIndex--;

		nextButton.interactable = true;
		endButton.interactable = true;

		if (_currentIconSetIndex == 0) {
			homeButton.interactable = false;
			backButton.interactable = false;
		}

		yield return (PopIconsOut ());
	}


	private IEnumerator LaunchGame () {

		yield return new WaitForSeconds (1);
		startButton.GetComponent<UnityEngine.UI.Button> ().interactable = true;
		startButton.interactable = false;
		SetAllIconsActiveState (false);
		RoundManager.S.boardManager.gridSize = GetGridSize ();
		RoundManager.S.StartGame (GetGameModeLogic(), GetButtonsBehaviours());
	}


	private IEnumerator NextIconSet () {
		yield return (PopIconsBack ());

		_currentIconSetIndex++;

		homeButton.interactable = true;
		backButton.interactable = true;

		if (_currentIconSetIndex == iconSets.Length) {
			nextButton.interactable = false;
			endButton.interactable = false;

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

		homeButton.interactable = true;
		backButton.interactable = true;
		nextButton.interactable = false;
		endButton.interactable = false;

		UpdateLabels ();
		yield return (iconsCover.StartColorBlend (true));
	}


	private void SetAllLabelsActiveState (bool value) {
		gameModeLabel.gameObject.SetActive (value);
		gameModeText.gameObject.SetActive (value);
		paceLabel.gameObject.SetActive (value);
		paceText.gameObject.SetActive (value);
		gridSizeLabel.gameObject.SetActive (value);
		gridSizeText.gameObject.SetActive (value);
		behavioursLabel.gameObject.SetActive (value);
		behavioursText.gameObject.SetActive (value);
		highscoreLabel.gameObject.SetActive (value);
		highscoreText.gameObject.SetActive (value);
	}


	private void UpdateLabels () {
		string tText = "";

		switch (_currentIconSetIndex) {
		case 0:		// GameMode Selection
			gameModeLabel.gameObject.SetActive (true);
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
			paceLabel.gameObject.SetActive (true);
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
			gridSizeLabel.gameObject.SetActive (true);
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
			behavioursLabel.gameObject.SetActive (true);
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

		// Highscore is always updated, so that (if showing) it will update accoding to the selection.
		highscoreText.text = (GetHighscore ()).ToString ();

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