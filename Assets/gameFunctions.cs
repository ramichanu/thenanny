using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gameFunctions : MonoBehaviour {
	// Use this for initialization
	void Start () {

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

}
