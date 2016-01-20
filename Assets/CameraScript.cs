using UnityEngine;
using System.Collections;

public class CameraScript : EventScript {

	public Transform target;
	public float distance = 10.0f;
	public float xSpeed = 10.0f;
	
	float rotationYAxis = 0.0f;
	
	float velocityX = 0.0f;


	public bool isRotating = false;
	float initialRotation;
	float limitRotationYAxis;
	float allRotationLimit;

	float cameraDistanceMax = 5f;
	float cameraDistanceMin = 3f;
	float cameraDistance = 3f;
	float scrollSpeed = 100f;

	float zoomInOut = (float)-0.1;

	float boundary = 2; 
	float screenWidth = 0;
	float screenHeight = 0;

	string isMovingTo = "";

	float speedMovement = 20f;
	
	// Use this for initialization
	void Start()
	{
		Vector3 angles = transform.eulerAngles;
		rotationYAxis = angles.y;
		initialRotation = rotationYAxis;
		limitRotationYAxis = rotationYAxis + 90;
		allRotationLimit = rotationYAxis + (90 * 4);
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}
	
	void Update()
	{
		zoomByMouseScrollWeel ();
		rotationCamera ();
		limitScreenMovement ();
	}

	void limitScreenMovement() {
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
		if ((Input.mousePosition.x < 0f + boundary || Input.GetKey("left") || Input.GetKey("a")) && transform.position.x > -4) {
			Camera.main.GetComponent<CameraScript> ().target = null;
			Vector3 toPosition = transform.TransformPoint(Vector3.left*speedMovement);
			StartCoroutine(moveToTargetSlowly(toPosition));
		}
		if ((Input.mousePosition.x > screenWidth - boundary || Input.GetKey("right") || Input.GetKey("d")) && transform.position.x < 10) {
			Camera.main.GetComponent<CameraScript> ().target = null;
			Vector3 toPosition = transform.TransformPoint(Vector3.right*speedMovement);
			StartCoroutine(moveToTargetSlowly(toPosition));
		}

		if ((Input.mousePosition.y > screenHeight - boundary || Input.GetKey("up") || Input.GetKey("w")) && transform.position.y < 8) {
			Camera.main.GetComponent<CameraScript> ().target = null;
			Vector3 toPosition = transform.TransformPoint(Vector3.up*speedMovement);
			StartCoroutine(moveToTargetSlowly(toPosition));
		}

		if ((Input.mousePosition.y < 0f + boundary || Input.GetKey("down")|| Input.GetKey("s")) && transform.position.y > -1) {
			Camera.main.GetComponent<CameraScript> ().target = null;
			Vector3 toPosition = transform.TransformPoint(Vector3.down*speedMovement);
			StartCoroutine(moveToTargetSlowly(toPosition));
		}
	}

	IEnumerator moveCameraToLeft(){

		Vector3 targetFrom = transform.position;
		Vector3 targetTo = targetFrom;
		targetTo.x -= 5; 

		while(transform.position.x > targetTo.x) {
			Debug.Log ("transPosX: " + transform.position.x + " targetToX: " + targetTo.x);
			transform.position = Camera.main.ViewportToWorldPoint(Vector3.Lerp (Camera.main.WorldToViewportPoint(transform.position), targetTo, Time.deltaTime * 0.1f));
			yield return null;
		}

	}

	IEnumerator moveToTargetSlowly(Vector3 targetPosition){
		targetPosition = Camera.main.WorldToScreenPoint (targetPosition);
		Vector3 initialPosition = Camera.main.WorldToScreenPoint (transform.position);
		transform.position = Camera.main.ScreenToWorldPoint(Vector3.Lerp(initialPosition, targetPosition, Time.deltaTime*0.3f));
		yield return null;
	}



	IEnumerator moveCameraToTarget(){
		float t = 0f;
		Vector3 initialPosition = transform.position;
		while(t < 1)
		{
			t += Time.deltaTime / 0.2f;
			transform.position = Vector3.Lerp(initialPosition, GameObject.Find ("player").transform.position, t);
			yield return null;
		}
		//Camera.main.GetComponent<CameraScript> ().target = GameObject.Find ("player").transform;
	}


	void rotationCamera() {
		Camera.main.orthographicSize = cameraDistance;
		if (target)
		{
			if(rotationYAxis> limitRotationYAxis){
				velocityX = 0.0f;
				if(rotationYAxis + 90 > allRotationLimit){
					limitRotationYAxis = initialRotation;
					rotationYAxis = transform.eulerAngles.y;
				} else {
					limitRotationYAxis = rotationYAxis + 90;
				}
				
				
			} else {
				rotationYAxis += velocityX;
				Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
				Quaternion toRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotationYAxis, 0);
				Quaternion rotation = toRotation;
				
				Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
				Vector3 position = rotation * negDistance + target.position;
				
				transform.rotation = rotation;
				transform.position = position;
				
				velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime);
			}
		}
	}

	void zoomByMouseScrollWeel(){
		if (Input.GetAxis ("Mouse ScrollWheel") != 0){
			zoomInOut = Input.GetAxis ("Mouse ScrollWheel");

			cameraDistance += zoomInOut * scrollSpeed;
			cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
		}
	}

	public void zoom(){

		float zoomPositiveOrNegative = (float)-0.1;
		if (zoomInOut == (float)-0.1) {
			zoomPositiveOrNegative = (float)0.1;
		} 

		zoomInOut = zoomPositiveOrNegative;
		cameraDistance += zoomPositiveOrNegative * scrollSpeed;
		cameraDistance = Mathf.Clamp (cameraDistance, cameraDistanceMin, cameraDistanceMax);
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	public void rotate(){
		velocityX += xSpeed * 1 *  0.04f;
	}

	IEnumerator moveCameraToPlayerSlowly(){
		float t = 0f;
		
		while(t < 1)
		{
			t += Time.deltaTime /1;
			Vector3 initialPosition = Camera.main.transform.position;
			Vector3 targetPosition = GameObject.Find("player").transform.position;
			Camera.main.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
			yield return null;
		}
		Camera.main.GetComponent<CameraScript> ().target = GameObject.Find("player").transform;
	}
	
	void moveCamToNanny(){
		StartCoroutine(moveCameraToPlayerSlowly());
		eventFinishedCallback("moveCamToNanny");
	}


}
