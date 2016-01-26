using UnityEngine;
using System.Collections;

public class CamFunctions: EventScript {

	public Transform target;
	public float distance = 10.0f;
	public float xSpeed = 10.0f;
	
	float rotationYAxis = 0.0f;
	
	float velocityX = 0.0f;


	public bool isRotating = false;
	public bool isCamFollowPlayer = false;

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
	public Ray ray;
	public RaycastHit hit;
	
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
		camFollowPlayer ();
		zoomByMouseScrollWeel ();
		rotationCamera ();
		limitScreenMovement ();
		ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);
	}

	void limitScreenMovement() {
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
		if ((Input.mousePosition.x < 0f + boundary || Input.GetKey("left") || Input.GetKey("a"))) {
			Camera.main.GetComponent<CamFunctions> ().target = null;
			isCamFollowPlayer = false;
			Vector3 toPosition = transform.TransformPoint(Vector3.left*speedMovement);
			StartCoroutine(moveToTargetSlowly(toPosition));
			outsideSceneThenMoveCam();
		}
		if ((Input.mousePosition.x > screenWidth - boundary || Input.GetKey("right") || Input.GetKey("d"))) {
			Camera.main.GetComponent<CamFunctions> ().target = null;
			isCamFollowPlayer = false;
			Vector3 toPosition = transform.TransformPoint(Vector3.right*speedMovement);
			StartCoroutine(moveToTargetSlowly(toPosition));
			outsideSceneThenMoveCam();
		}

		if ((Input.mousePosition.y > screenHeight - boundary || Input.GetKey("up") || Input.GetKey("w"))) {
			Camera.main.GetComponent<CamFunctions> ().target = null;
			isCamFollowPlayer = false;
			Vector3 toPosition = transform.TransformPoint(Vector3.up*speedMovement);
			StartCoroutine(moveToTargetSlowly(toPosition));
			outsideSceneThenMoveCam();
		}

		if ((Input.mousePosition.y < 0f + boundary || Input.GetKey("down")|| Input.GetKey("s"))) {
			Camera.main.GetComponent<CamFunctions> ().target = null;
			isCamFollowPlayer = false;
			Vector3 toPosition = transform.TransformPoint(Vector3.down*speedMovement);
			StartCoroutine(moveToTargetSlowly(toPosition));
			outsideSceneThenMoveCam();
		}
	}

	void outsideSceneThenMoveCam(){
		if (!Physics.Raycast(ray, out hit)) {
			moveCamToNanny();
		}
	}

	IEnumerator moveCameraToLeft(){

		Vector3 targetFrom = transform.position;
		Vector3 targetTo = targetFrom;
		targetTo.x -= 5; 

		while(transform.position.x > targetTo.x) {
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
		if (target && isRotating)
		{
			if(rotationYAxis> limitRotationYAxis){
				velocityX = 0.0f;
				isRotating = false;
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

	public void rotateEvent(){
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		methodsToCall.Add("camera_rotate");
		
		base.eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	public void rotate(){

		velocityX += xSpeed * 1 *  0.04f;
		isRotating = true;
		Camera.main.GetComponent<CamFunctions> ().target = GameObject.Find("player").transform;

		eventFinishedCallback("rotate");
	}

	public void eventCameraToNanny(){
		if (Camera.main.GetComponent<CamFunctions> ().target == null) {
			ArrayList canInterruptBy = new ArrayList ();
			ArrayList methodsToCall = new ArrayList ();
			ArrayList methodsAfterInterrupt = new ArrayList ();

			methodsToCall.Add ("camera_moveCamToNanny");

			ArrayList methodsDisabledUntilEventFinished = new ArrayList ();
			eventDisp.addEvent (methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
		}
	}
	
	void moveCamToNanny(){
		StartCoroutine(moveCameraToPlayerSlowly());
		eventFinishedCallback ("moveCamToNanny");
	}

	IEnumerator moveCameraToPlayerSlowly(){
		float t = 0f;
		
		while(t < 1)
		{
			t += Time.deltaTime /1;
			Vector3 initialPosition = Camera.main.transform.position;
			Vector3 targetPosition = GameObject.Find("player").transform.position;
			Camera.main.transform.position = Vector3.Lerp(initialPosition, targetPosition + Camera.main.transform.forward * -10f, 3 * Time.deltaTime);
			yield return null;
		}

		enableCamFollowPlayer ();

	}

	void enableCamFollowPlayer() {
		isCamFollowPlayer = true;
		eventFinishedCallback ("enableCamFollowPlayer");
	}

	void camFollowPlayer() {
		if (isCamFollowPlayer) {
			transform.position = Vector3.Lerp (transform.position, GameObject.Find ("player").transform.position + Camera.main.transform.forward * -10f, 3 * Time.deltaTime);
		}
	}

	void disableCamFollowPlayer() {
		StopAllCoroutines ();
		isCamFollowPlayer = false;
		eventFinishedCallback ("disableCamFollowPlayer");
	}
}
