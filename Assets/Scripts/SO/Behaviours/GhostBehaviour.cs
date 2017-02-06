using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Buttons Behaviours/Ghosts", fileName = "Ghost Behaviour")]
public class GhostBehaviour : ButtonsBehaviour {

	[Header ("Ghost Effect")]
	public Color hideColor;
	public float hideDuration = 0.5f;
	public Color appearColor;
	public float appearDuration = 0.3f;
	public float maxButtonsOffset = 0.25f;

	private Color usableHideColor;
	private Color usableAppearColor;

	private bool _initialized = false;
	private bool _started = false;
	private CircleButton[] buttons;
	private Color[] buttonsStartColors;
	private float[] buttonsAnimationOffset;
	private float startTime;
	private float pauseDurations;


	private void OnEnable () {
		_initialized = false;
		_started = false;
		buttons = null;
		buttonsStartColors = null;
		buttonsAnimationOffset = null;
		startTime = 0f;
		pauseDurations = 0f;

		usableHideColor = hideColor;
		usableAppearColor = appearColor;
	}


	public override void InitializeBehaviour (CircleButton[] buttons) {
		OnEnable ();
		this.buttons = buttons;

		buttonsStartColors = new Color[buttons.Length];
		for (int i = 0; i < this.buttons.Length; i++) {
			buttonsStartColors [i] = this.buttons [i].GetButtonColor ();
		}

		buttonsAnimationOffset = new float[buttons.Length];
		for (int i = 0; i < this.buttons.Length; i++) {
			buttonsAnimationOffset [i] = Random.Range (0f, maxButtonsOffset);
		}

		usableHideColor *= buttonsStartColors [0];
		usableAppearColor *= buttonsStartColors [0];
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

		pauseDurations += unpauseDuration;

		float elapsedTime = Time.time - startTime - pauseDurations;
		float cycleTime = elapsedTime % (hideDuration + appearDuration);

		float u;
		Color currentColor;

		if (maxButtonsOffset == 0) {
			//	Use if there is no offset i Animation
			if (cycleTime <= hideDuration) {
				// Hide
				u = cycleTime / hideDuration;
				currentColor = Color.LerpUnclamped (usableAppearColor, usableHideColor, u);
			} else {
				// Appear
				u = (cycleTime - hideDuration) / appearDuration;
				currentColor = Color.LerpUnclamped (usableHideColor, usableAppearColor, u);
			}

			for (int i = 0; i < buttons.Length; i++) {
				if (!buttons [i].isAnimating) {
					buttons [i].SetButtonColor (currentColor);
					buttons [i].SetLightAlphaMultiplier (currentColor.a);
				}
			}
		} else {
			// Used if there IS offset in Animation

			float buttonTime;
			for (int i = 0; i < buttons.Length; i++) {
				buttonTime = cycleTime + buttonsAnimationOffset [i];
				if (buttonTime <= hideDuration) {
					// Hide
					u = buttonTime / hideDuration;
					currentColor = Color.LerpUnclamped (usableAppearColor, usableHideColor, u);
				} else {
					// Appear
					u = (buttonTime - hideDuration) / appearDuration;
					currentColor = Color.LerpUnclamped (usableHideColor, usableAppearColor, u);
				}
				
				if (!buttons [i].isAnimating) {
					buttons [i].SetButtonColor (currentColor);
					buttons [i].SetLightAlphaMultiplier (currentColor.a);
				}
			}
		}

	}


	void StartBehaviour () {
		startTime = Time.time;
		pauseDurations = 0f;
		_started = true;

		buttonsAnimationOffset = new float[buttons.Length];
		for (int i = 0; i < this.buttons.Length; i++) {
			buttonsAnimationOffset [i] = Random.Range (0f, maxButtonsOffset);
		}
	}

	public override void StopBehaviour () {
		for (int i = 0; i < buttons.Length; i++) {
			buttons [i].SetButtonColor (buttonsStartColors[i]);
		}
		_started = false;
	}
}
