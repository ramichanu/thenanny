using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gameFunctions : MonoBehaviour {
	public int initialCountDown = 50;
	public int countdown;
	public Text totalMoney;
	// Use this for initialization
	void Start () {
		totalMoney.text = PlayerPrefs.GetInt ("totalMoney").ToString ();
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
				GameObject child = GameObject.Find ("child");

				int lives = child.GetComponent<childController>().lives;
				int initialLives = child.GetComponent<childController>().initialLives;

				PlayerPrefs.SetInt("lives", lives);
				PlayerPrefs.SetInt("initialLives", initialLives);

				Application.LoadLevel("endMenu");
			}
			GameObject.Find ("countdown").GetComponent<Text>().text = countdown.ToString();
		}
	}



}
