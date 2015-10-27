using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

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
	
	// Use this for initialization
	void Start()
	{
		Vector3 angles = transform.eulerAngles;
		rotationYAxis = angles.y;
		initialRotation = rotationYAxis;
		limitRotationYAxis = rotationYAxis + 90;
		allRotationLimit = rotationYAxis + (90 * 4);




	}
	
	void Update()
	{

		zoomByMouseScrollWeel ();
		
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


}
