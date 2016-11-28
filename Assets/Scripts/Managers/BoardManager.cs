using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {
	public GameObject buttonPrefab;
	[Range (3,5)]
	public int gridSize = 4;
	[Range (0,1)]
	public float edgePadding = 0.25f;
	[Range (0.1f,1)]
	public float buttonScaleToSpaceRatio = 2f / 3f;

	private Transform buttonParent;
	private float buttonScale = 1f;


	private Button[] buttons;
	private Vector3[] buttonPositions;


	public void SetUpGrid () {
		ClearGrid ();
		SetButtonScale ();
		SetParentPosition ();
		CreateButtons ();
	}


	public Button[] GetButtons () {
		return buttons;
	}


	public Vector3[] GetButtonPositions () {
		return buttonPositions;
	}


	public void ClearGrid () {
		buttons = null;
		if (buttonParent != null) {
			Destroy (buttonParent.gameObject);
			buttonParent = null;
		}
	}


	private void SetButtonScale () {
		// SetButtonScale calculates the size of the buttons based on the width of the camara (which depends on the aspect ratio of the screen the game is played in)

		// Get the width of the screen in game units
		float screenWidth = 2 * Camera.main.orthographicSize * Camera.main.aspect;

		//	Get the scale of the buttons. Each button will have some free space around itself (depending on the buttonToSpaceRatio),
		//	either to the screen edge (+ an edgePadding) or to another button.
		//	So, each button occupies and extra radius besides the 2 radius it contains.
		//	But, we must substract the edgePadding from the available screenWidth.
		float widthForButton = (screenWidth - 2* edgePadding) / (float)gridSize;
		//	The scale is only 2/3 of the allocated widthForButton, since 1 radius is just empty space around
		float scale = widthForButton * buttonScaleToSpaceRatio;	

		buttonScale = scale;
	}


	private void SetParentPosition () {
		// Create the buttonParent object if it does not exist
		if (buttonParent == null) {
			buttonParent = (new GameObject ("Buttons")).transform;
		}

		// Calculate the space that each button needs
		float spacePerButton = buttonScale / buttonScaleToSpaceRatio;

		//Calculate the distance from the bottom of the screen (keeping the edgePadding in mind)
		float distanceFromBottomOfScreen = spacePerButton * gridSize/2f + edgePadding;

		float yPos = -(Camera.main.orthographicSize - distanceFromBottomOfScreen);

		buttonParent.transform.position = Vector3.up * yPos;
	}


	private void CreateButtons () {
		// Calculate the space that each button needs. This will be used later
		float spacePerButton = buttonScale / buttonScaleToSpaceRatio;

		// All buttons will be created as children of buttonParent which will be right in the middle of the grid
		// To calculate the local position of each button, we'll start by calculating the position of the grid bottom-left edge
		// The edgePadding is not taken into account, because the button's positions will be set local to the buttonParent.
		float edgeOffset = (gridSize / 2f) * spacePerButton;
		Vector2 bottomLeftEdge = new Vector2 (-edgeOffset, -edgeOffset);

		// Next, we'll calculate the position of the first button (0,0) as the reference position for all buttons
		Vector2 referencePosition = bottomLeftEdge + new Vector2 (spacePerButton/2, spacePerButton/2);

		// Initialize the buttons array and the buttonsPositions array to the right size.
		buttons = new Button[gridSize * gridSize];
		buttonPositions = new Vector3[gridSize * gridSize];

		// Now we can create all buttons
		float screenWidth = 2 * Camera.main.orthographicSize * Camera.main.aspect;
		GameObject button;
		Vector2 positionOffset;
		Mover bMover;		// the Mover is used for the EndGame animation where all buttons move to the center of the buttonsGrid after loss.
		Vector3 moveVector;
		for (int y = 0; y < gridSize; y++) {
			for (int x = 0; x < gridSize; x++) {
				button = Instantiate (buttonPrefab);
				button.transform.SetParent (buttonParent);
				positionOffset = new Vector2 (x * spacePerButton, y * spacePerButton);
				button.transform.localPosition = referencePosition + positionOffset;
				button.transform.localScale = Vector3.one * buttonScale;
				buttons[y * gridSize + x] = button.GetComponent<Button> ();
				buttonPositions [y * gridSize + x] = button.transform.position;

				bMover = button.GetComponent<Mover> ();
				bMover.startPosition = button.transform.position;

				moveVector = (button.transform.position - buttonParent.transform.position).normalized;
				moveVector *= screenWidth;
				bMover.targetPosition = buttonParent.transform.position + moveVector;
			}
		}
	}

}
