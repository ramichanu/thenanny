using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	
	public Transform target;
	float yPos;
	Quaternion rotation;

	void Start() {
		yPos = transform.position.y;
		rotation = transform.rotation;
	}
	
	void Update () {
		transform.position = new Vector3(target.position.x+3, yPos, target.position.z-4);
		transform.LookAt (target);

		Camera.main.transform.rotation = rotation;
	}
}
