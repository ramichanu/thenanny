using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LifeAndHunger : MonoBehaviour {

	public float totalLife;
	public float currentLife;

	private ArrayList hearts = new ArrayList();
	private GameObject lifeAndHunger;
	private Vector3 lifeAndHungerPosition;

	private int totalHeartsCount;
	private float center = 0;

	// Use this for initialization
	void Start () {
		NotificationCenter.DefaultCenter.AddObserver(this, "executeScript");
		currentLife = totalLife;
		lifeAndHunger = GameObject.Find("lifeAndHunger");
		lifeAndHungerPosition = lifeAndHunger.transform.position;
		printAllHearts ();


	}
	
	// Update is called once per frame
	void Update () {
		int uno = 1;
	}

	void printAllHearts() {

		totalHeartsCount = (int)(currentLife * 10 / totalLife);
		string type = "red";

		for(int i=1; i<=10; i++){
			if(i > totalHeartsCount){
				type = "black";
			}
			insertHeart (type);
		}

		GameObject canvas = GameObject.Find("Panel");
		if (center == 0) {
			center = getHeartsCenter ();
		}
		int screenCenter = (int) (Screen.width / 2 + center);
		int lifeAndHungerX = (int)(lifeAndHunger.transform.position.x);
		if (screenCenter != lifeAndHungerX) {
			lifeAndHunger.transform.position = new Vector3 (Screen.width/2+center, lifeAndHunger.transform.position.y, lifeAndHunger.transform.position.z);
		}
	}

	void insertHeart(string type) {

		GameObject heart = Instantiate(Resources.Load("hub/heart")) as GameObject;
		string sprite = "heart_" + type;
		heart.GetComponent<Image>().sprite = Resources.Load<Sprite>("imgs/hub/" + sprite);
		heart.transform.SetParent (lifeAndHunger.transform);
		heart.transform.position = new Vector3(Screen.width/2, lifeAndHunger.transform.position.y, lifeAndHunger.transform.position.z);

		if (hearts.Count > 0) {
			GameObject lastHeart = (GameObject)hearts[hearts.Count-1];
			heart.transform.position = new Vector3(lastHeart.transform.position.x + 20, lifeAndHunger.transform.position.y, lifeAndHunger.transform.position.z);
		}

		hearts.Add(heart);
	}

	float getHeartsCenter(){
		GameObject firstHeart = (GameObject)hearts [0];
		GameObject lastHeart = (GameObject)hearts[hearts.Count-1];

		float center = (firstHeart.transform.position.x - lastHeart.transform.position.x)/2;
		return center;
	}

	void destroyAllHearts(){
		GameObject[] heartsArray= GameObject.FindGameObjectsWithTag("heart");
		foreach (GameObject heart in heartsArray) {
			Destroy(heart);
		}
		hearts.Clear ();
		lifeAndHunger.transform.position = lifeAndHungerPosition;
	}

	public void restPercentLife(int percent){
		currentLife -= (percent / totalLife) * 100;
		destroyAllHearts ();
		printAllHearts ();
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

}
