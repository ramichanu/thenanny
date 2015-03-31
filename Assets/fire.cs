using UnityEngine;
using System.Collections;

public class fire : MonoBehaviour {

	public GameObject parent;
	public bool isCollidingWithObstacle = false;
	public bool isSurroundedByFires = false;
	public bool isFireEnabled = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collision) {
		if (collision.transform.tag == "wall") {
			isCollidingWithObstacle = true;
		}
		
	}
}
