using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Door : MonoBehaviour {

	public Ray ray;
	public RaycastHit hit;
	public GameObject forward = new GameObject();
	public Plane planeDoor = new Plane();
	public bool isTouchingDoor = false;
	public bool isDoorMovementActive = false;

	Vector3 from ;
	Quaternion to;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void rotateDoor(GameObject character, float degrees, bool isOpen) {
		if(!isDoorMovementActive) {
			StartCoroutine (rotatingDoor(character, degrees, isOpen, 1));
		}

	}

	IEnumerator rotatingDoor(GameObject character, float degrees, bool isOpen, float forwardDir) {
		isDoorMovementActive = true;

		if (Vector3.Dot (forward.transform.forward, character.transform.forward)>0 && !isOpen) {
			forwardDir = -1;
		} 

		float distanceToRotation = 0;
		from = transform.eulerAngles;
		to = Quaternion.Euler (transform.eulerAngles + ((new Vector3(0,0,degrees) * forwardDir)));

		do {

			transform.rotation = Quaternion.Slerp(transform.rotation, to, Time.deltaTime * 7);
			distanceToRotation = Vector3.Distance(to.eulerAngles, transform.eulerAngles);
			yield return null;
		}while (distanceToRotation > 2);

		while(isTouchingDoor) {
			yield return null;
		}

		if (!isOpen) {
			StartCoroutine (rotatingDoor(character, -90, true, forwardDir *1));
		} else {
			isDoorMovementActive = false;
		}
	}

	void OnTriggerExit(Collider col){
		switch (col.transform.tag){
		case "player":
		case "child":
		case "dog":
		case "cat":
		case "madLady":
			isTouchingDoor = false;
			break;
			//Debug.Log ("NOP");
		} 
	}

	void OnTriggerEnter(Collider col){
		switch (col.transform.tag){
		case "player":
		case "child":
		case "dog":
		case "cat":
		case "madLady":
			//Debug.Log ("YES");
			isTouchingDoor = true;
			rotateDoor(col.transform.gameObject, 90, false);
			break;
		} 
	}
}
