﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Specialized;
using UnityEngine.EventSystems;

public class gameFunctions : EventScript {
	public int initialCountDown = 50;
	public int countdown;
	public Text totalMoney;
	public string lastButtonClicked = "";
	RaycastHit hit;
	EventDispatcher eventDisp;


	public GameObject menu = null;
	// Use this for initialization
	void Start () {
		//totalMoney.text = PlayerPrefs.GetInt ("totalMoney").ToString ();
		eventDisp = EventDispatcher.DefaultEventDispatcher;
		countdown = initialCountDown;

		GameObject.Find ("countdown").GetComponent<Text>().text = countdown.ToString();
		InvokeRepeating ("restCountdown", 1, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		clickEffectTerrain ();
	}

	public void pauseGame () {
		if (Time.timeScale == 0) {
			Time.timeScale =1;
		} else {
			Time.timeScale =0;
		}
	}

	void restCountdown(){
		if (countdown > 0) {
			countdown -= 1;
			GameObject.Find ("countdown").GetComponent<Text>().text = countdown.ToString();
		} else {
			GameObject lifeHungerBar = GameObject.Find ("lifeHungerBar");
			
			int lives = (int)lifeHungerBar.GetComponent<NewLifeAndHunger>().currentLife;
			int initialLives = (int)lifeHungerBar.GetComponent<NewLifeAndHunger>().totalLife;
			
			PlayerPrefs.SetInt("lives", lives * 100);
			PlayerPrefs.SetInt("initialLives", initialLives * 100);
			
			Application.LoadLevel("endMenu");
		}
	}

	public void createClickMenu(GameObject hit){
		ArrayList buttonOptions = new ArrayList();
		Vector3 buttonPos = new Vector3 ();

		if(hit.transform != null){
			switch(hit.transform.tag){
				case "child":
					if(hit.transform.gameObject.GetComponent<ChildControllerNew>().isOutside) {
						buttonOptions.Add("goBack");
						buttonPos = Camera.main.WorldToScreenPoint(hit.transform.position);
						break;
					}

					if(hit.transform.gameObject.GetComponent<ChildControllerNew>().isElectrifying) {
						buttonOptions.Add("helpElectrifying");
						buttonPos = Camera.main.WorldToScreenPoint(hit.transform.position);
						break;
					}

					if(hit.transform.gameObject.GetComponent<ChildControllerNew>().isBurning) {
						buttonOptions.Add("helpBurning");
					}

					bool playerHasBabbyBottle = GameObject.Find("player").GetComponent<PlayerMovementNew>().hasBabyBottle;
					if(playerHasBabbyBottle) {
						buttonOptions.Add("feed");
					}
				buttonPos = Camera.main.WorldToScreenPoint(hit.transform.position);
					
				break;
				case "madLady":
				buttonOptions.Add("kickOut");
				buttonPos = Camera.main.WorldToScreenPoint(hit.transform.position);
				break;
				case "player":
				buttonOptions.Add("putDown");
				buttonPos = Input.mousePosition;
				break;
			}
			

			
			if(buttonOptions.Count > 0){
				//to stop child
				GameObject canvas = GameObject.Find ("Canvas");
				canvas.GetComponent<gameFunctions> ().pauseGame ();
				disableClickEvent();

				menu = new GameObject();
				menu.transform.name = "menu";
				menu.transform.SetParent(canvas.transform, false);
			}
			foreach(string buttonOption in buttonOptions)
			{
				showMenuByPositionAndButtonType(buttonPos, buttonOption, hit);
				buttonPos.y += 30;
				
			}
		}
	}

	void showMenuByPositionAndButtonType(Vector3 position, string buttonType, GameObject hit) {
		
		OrderedDictionary buttonStringType = new OrderedDictionary ();

		switch(hit.transform.tag){
			case "child":
				buttonStringType.Add("feed", "Alimentar");
				buttonStringType.Add("helpBurning", "Apagar fuego");
				buttonStringType.Add("helpElectrifying", "Apartar");
				buttonStringType.Add("goBack", "Vuelve a casa");
				
				break;
			case "madLady":
				buttonStringType.Add("kickOut", "¡Fuera!");
			break;
			case "player":
				buttonStringType.Add("putDown", "Dejar en el suelo");
			break;
		}
		
		GameObject button = Instantiate(Resources.Load("hub/button")) as GameObject;
		string buttonText = (string)buttonStringType[buttonType];
		button.name = buttonType;
		button.transform.FindChild("Text").GetComponent<Text>().text = buttonText;
		button.transform.SetParent (menu.transform, false); 
		
		button.GetComponent<Button> ().onClick.AddListener (() => {
			Destroy (GameObject.Find ("menu"));
			callSelectedOption(hit, buttonType);
			lastButtonClicked = buttonType;
			GameObject canvas = GameObject.Find ("Canvas");
			canvas.GetComponent<gameFunctions> ().pauseGame ();
			
		});
		
		button.transform.position = new Vector2 (position.x, position.y);
		
	}

	void callSelectedOption(GameObject hit, string option){
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		methodsToCall.Add (hit.transform.tag + "_" + option);

		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void disableClickEvent() {
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		
		methodsToCall.Add("player_disableClick");
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void clickEffectTerrain(){
		if (Input.GetMouseButtonDown (0)) {
			bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();
			if(isOverUI) {
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			int layerMask = 1 << 8;
			layerMask = ~layerMask;
			
			
			if (Input.touchCount > 0) {
				isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId);
			}

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask) && !isOverUI) {
				ArrayList canInterruptBy = new ArrayList();
				ArrayList methodsToCall = new ArrayList();
				ArrayList methodsAfterInterrupt = new ArrayList();
				ArrayList methodsDisabledUntilEventFinished = new ArrayList();

				switch (hit.transform.tag) {
				case "player":
				case "terrain":
				case "brokenGlass":
				case "terrainHome":
					StartCoroutine("clickEffect");
					break;
				}
			}

		}
	}

	IEnumerator clickEffect(){
		GameObject clickEffect = (GameObject)GameObject.Instantiate(Resources.Load("scenary/clickEffectTerrain"), hit.point + new Vector3(0f, 0.03f, 0f), Quaternion.identity);
		yield return new WaitForSeconds(1);
		Destroy(clickEffect);
	}


}
