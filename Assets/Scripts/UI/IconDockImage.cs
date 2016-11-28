using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class IconDockImage : MonoBehaviour, IPointerClickHandler {

	[Header ("Call: MenuController.OnIconClicked(iconIndex)")]
	public MenuController menuController;
	public int iconIndex;

	[Header ("Call iconDocksGroup.OnIconDockClicked (image)")]
	public IconDocksGroup iconDocksGroup;

	private UnityEngine.UI.Image image;


	void Awake () {
		image = GetComponent<UnityEngine.UI.Image> ();
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		menuController.OnIconClicked (iconIndex);
		iconDocksGroup.OnIconDockClicked (image);
	}

}
