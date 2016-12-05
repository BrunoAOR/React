using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class IconDockImage : MonoBehaviour, IPointerClickHandler {

	public Image lockImage;

	private bool _isUnlocked;

	[Header ("Call: MenuController.OnIconClicked(iconIndex)")]
	public MenuController menuController;
	public int iconIndex;

	[Header ("Call iconDocksGroup.OnIconDockClicked (image)")]
	public IconDocksGroup iconDocksGroup;

	private UnityEngine.UI.Image _image;


	void Reset () {
		if (lockImage == null) {
			Image [] images = GetComponentsInChildren<Image> ();
			for (int i = 0; i < images.Length; i++) {
				if (images [i].gameObject.GetInstanceID () != gameObject.GetInstanceID ()) {
					lockImage = images [i];
					break;
				}
			}
		}
	}

	void Awake () {
		_image = GetComponent<UnityEngine.UI.Image> ();

		if (lockImage == null) {
			Image [] images = GetComponentsInChildren<Image> ();
			for (int i = 0; i < images.Length; i++) {
				if (images [i].gameObject.GetInstanceID () != gameObject.GetInstanceID ()) {
					lockImage = images [i];
					break;
				}
			}
		}

	}

	public void OnPointerClick (PointerEventData eventData)
	{
		menuController.OnIconClicked (iconIndex, _isUnlocked);

		if (_isUnlocked) {
			iconDocksGroup.OnIconDockClicked (_image);
		}
	}

	public void UpdateUnlockState (bool shouldUnlock) {
		if (shouldUnlock) {
			Unlock ();
		} else {
			Lock ();
		}
	}

	public void Unlock () {
		if (_image == null)
			_image = GetComponent<UnityEngine.UI.Image> ();
		
		_image.enabled = true;
		lockImage.gameObject.SetActive (false);
		_isUnlocked = true;

	}

	public void Lock () {
		if (_image == null)
			_image = GetComponent<UnityEngine.UI.Image> ();
		
		_image.enabled = false;
		lockImage.gameObject.SetActive (true);
		_isUnlocked = false;
	}

}
