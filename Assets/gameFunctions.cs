using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Specialized;

public class gameFunctions : MonoBehaviour {
	public int initialCountDown = 50;
	public int countdown;
	public Text totalMoney;
	RaycastHit hit;
	EventDispatcher eventDisp;


	public GameObject menu = null;
	// Use this for initialization
	void Start () {
		NotificationCenter.DefaultCenter.AddObserver(this, "executeScript");
		//totalMoney.text = PlayerPrefs.GetInt ("totalMoney").ToString ();
		eventDisp = EventDispatcher.DefaultEventDispatcher;
		countdown = initialCountDown;

		GameObject.Find ("countdown").GetComponent<Text>().text = countdown.ToString();
		InvokeRepeating ("restCountdown", 1, 1f);
	}
	
	// Update is called once per frame
	void Update () {
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
			if(countdown == 0){
				GameObject child = GameObject.Find ("lifeAndHunger");

				int lives = (int)child.GetComponent<LifeAndHunger>().currentLife;
				int initialLives = (int)child.GetComponent<LifeAndHunger>().totalLife;

				PlayerPrefs.SetInt("lives", lives);
				PlayerPrefs.SetInt("initialLives", initialLives);

				Application.LoadLevel("endMenu");
			}
			GameObject.Find ("countdown").GetComponent<Text>().text = countdown.ToString();
		}
	}

	public void createClickMenu(GameObject hit){
		ArrayList buttonOptions = new ArrayList();
		if(hit.transform != null){
			switch(hit.transform.tag){
				case "child":
					if(hit.transform.gameObject.GetComponent<ChildControllerNew>().state == ChildControllerNew.BURNING) {
						buttonOptions.Add("helpBurning");
					}

					bool playerHasBabbyBottle = GameObject.Find("player").GetComponent<PlayerMovementNew>().hasBabyBottle;
					if(playerHasBabbyBottle) {
						buttonOptions.Add("feed");
					}
					
				break;
			}
			
			Vector3 buttonPos = Camera.main.WorldToScreenPoint(hit.transform.position);
			
			if(buttonOptions.Count > 0){
				//to stop child

				GameObject canvas = GameObject.Find ("Canvas");
				canvas.GetComponent<gameFunctions> ().pauseGame ();
				
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
				buttonStringType.Add("feed", "Dar biberon");
				buttonStringType.Add("helpBurning", "Echar agua");
				
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

	void eventFinishedCallback(string methodExecuted){
		Hashtable options = new Hashtable ();
		string methodCalled = transform.name + "_" + methodExecuted;
		options.Add ("methodCalled", methodCalled);
		NotificationCenter.DefaultCenter.PostNotification(this, "eventIsFinished", options);
	}
	
	void executeScript(Notification options){
		
		if(options.data["objectName"].ToString() == transform.name)
		{
			Invoke(options.data["scriptMethod"].ToString(), 0);
		}
	}


}
