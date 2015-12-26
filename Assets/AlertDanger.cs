using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AlertDanger : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler{
	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	// Use this for initialization
	void Start () {

	}
	public void OnPointerEnter(PointerEventData eventData) {
		Cursor.SetCursor(this.cursorTexture, Vector2.zero, CursorMode.Auto);
	}
	public void OnPointerExit(PointerEventData eventData) {
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}

	public void OnPointerDown(PointerEventData eventData) {
		//Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}

}
