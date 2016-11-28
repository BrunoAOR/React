using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Buttons Behaviours/Motion", fileName = "Motion Behaviour")]
public class MotionBehaviour : ButtonsBehaviour {

	[Header ("Motion Effect")]
	public float stopInPointDuration = 0.5f;
	public float pointToPointDuration = 1f;
	public MotionPaths motionPathsHandler;

	private bool _initialized = false;
	private bool _started = false;
	private Button[] buttons;
	private int[] buttonsCurrentGridPositionIndex;
	private Vector3[] gridPositions;
	private Path gridPath;
	private float startTime;
	private float pauseDurations;
	private float previousCyclesTime;

	private void OnEnable () {
		_initialized = false;
		_started = false;
		buttons = null;
		buttonsCurrentGridPositionIndex = null;
		gridPositions = null;
		gridPath = null;
		startTime = 0f;
		pauseDurations = 0f;
		previousCyclesTime = 0f;
	}


	public override void InitializeBehaviour (Button[] buttons) {
		OnEnable ();
		this.buttons = buttons;

		gridPositions = new Vector3[buttons.Length];
		for (int i = 0; i < this.buttons.Length; i++) {
			gridPositions [i] = this.buttons [i].transform.position;
		}

		buttonsCurrentGridPositionIndex = new int[buttons.Length];
		for (int i = 0; i < this.buttons.Length; i++) {
			buttonsCurrentGridPositionIndex [i] = i;
		}

		gridPath = motionPathsHandler.GetPath (RoundManager.S.boardManager.gridSize);

		if (gridPath == null) {
			Debug.LogWarning ("Behaviour.InitializeBehaviour could NOT find a valid gridPath!");
			return;
		}

		_initialized = true;
	}


	public override void RunBehaviour (float unpauseDuration = 0) {
		if (!_initialized) {
			Debug.LogWarning ("Behaviour.InitializeBehaviour has not been called before attempting to call RunBehaviour!");
			return;
		}

		if (!_started) {
			StartBehaviour ();
		}

		if (pointToPointDuration == 0) {
			return;
		}

		pauseDurations += unpauseDuration;

		float elapsedTime = Time.time - startTime - pauseDurations - previousCyclesTime;

		if (elapsedTime > (stopInPointDuration + pointToPointDuration)) {
			// Go to next cycle
			previousCyclesTime += stopInPointDuration + pointToPointDuration;
			elapsedTime -= stopInPointDuration + pointToPointDuration;

			// Adjust the buttonsCurrentPositionIndex to their next position (based on the gridPath);
			for (int i = 0; i < buttonsCurrentGridPositionIndex.Length; i++) {
				buttonsCurrentGridPositionIndex [i] = gridPath.motions [buttonsCurrentGridPositionIndex[i]];
			}
		}

		float u;
		Vector3 currentPosition;

		for (int i = 0; i < buttons.Length; i++) {
			if (elapsedTime <= stopInPointDuration) {
				// Stop in place
				currentPosition = gridPositions [buttonsCurrentGridPositionIndex [i]];
			} else {
				// Move
				u = (elapsedTime - stopInPointDuration) / pointToPointDuration;
				currentPosition = Vector3.Lerp (gridPositions[buttonsCurrentGridPositionIndex[i]], gridPositions[gridPath.motions[buttonsCurrentGridPositionIndex[i]]],u);
			}

			buttons [i].transform.position = currentPosition;
		}
				
	}


	void StartBehaviour () {
		startTime = Time.time;
		pauseDurations = 0f;
		previousCyclesTime = 0f;

		_started = true;
	}


	public override void StopBehaviour () {
		_started = false;
	}
}
