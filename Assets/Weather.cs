using UnityEngine;
using System.Collections;

public class Weather : MonoBehaviour {

	public GameObject rain = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void raining(bool active) {
		rain.SetActive(active);
	}
}
