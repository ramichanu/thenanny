using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class NewLifeAndHunger : MonoBehaviour {

	bool isStarving = false;

	public float totalLife = 1;
	public float currentLife;
	
	private ArrayList hearts = new ArrayList();
	private GameObject lifeAndHunger;
	private Vector3 lifeAndHungerPosition;
	
	private int totalHeartsCount;
	private float center = 0;
	public float hunger = 1;
	
	// Use this for initialization
	void Start () {
		NotificationCenter.DefaultCenter.AddObserver(this, "executeScript");
		currentLife = totalLife;
		setHungerBarPerSecond (0.08f);
		
	}
	
	// Update is called once per frame
	void Update () {
		int uno = 1;
	}

	
	public void restPercentLive(int percent){
		currentLife -= (percent / totalLife) * 100;
		thereIsNoLives();
	}
	void thereIsNoLives(){
		if (currentLife <= 0) {
			goToMenu("mainMenu");
		}
	}
	
	void goToMenu(string menuName){
		Application.LoadLevel(menuName);
	}
	
	void setHungerBarPerSecond(float seconds) {
		InvokeRepeating("setHungerBar", 2, seconds);
	}
	
	void setHungerBar(){
		hunger -= 0.0016f;
		
		GameObject hungerBar = GameObject.Find("hungerBar");
		hungerBar.GetComponent<Image> ().fillAmount = hunger;
		
		if (hungerBar.GetComponent<Image> ().fillAmount == 0) {
			if(!isStarving) {
				isStarving = true;
				InvokeRepeating("childStarving", 0f, 2f);
			}
		}	
	}

	void childStarving(){
		GameObject child = GameObject.Find ("child");
		child.GetComponent<ChildControllerNew>().hitAndPain(0.03f);
	}

	public void restLife(float restPercent){
		totalLife -= restPercent;
		
		GameObject lifeBar = GameObject.Find("lifeBar");
		lifeBar.GetComponent<Image> ().fillAmount = totalLife;

		if (lifeBar.GetComponent<Image> ().fillAmount <= 0) {
			goToMenu("mainMenu");
		}	
	}
	
	public void resetHunger() {
		Sprite withBabyBottle =  Resources.Load <Sprite>("imgs/hub/babyBottleNOT");
		if (withBabyBottle){
			GameObject.Find ("babyBottleIcon").GetComponent<Image>().sprite = withBabyBottle;
		} else {
			Debug.LogError("Sprite not found", this);
		};
		GameObject.Find ("babyBottleIcon").GetComponent<Image>().sprite = withBabyBottle;
		
		GameObject hungerBar = GameObject.Find("hungerBar");
		hungerBar.GetComponent<Image> ().fillAmount = 1;
		hunger = 1;
		
	}
	
}
