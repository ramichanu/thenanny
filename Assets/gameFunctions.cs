using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gameFunctions : MonoBehaviour {
	public int countdown = 150;
	// Use this for initialization
	void Start () {
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
				Application.LoadLevel("endMenu");
			}
			GameObject.Find ("countdown").GetComponent<Text>().text = countdown.ToString();
		}
	}



}
