using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public Transform target;
	public float distance = 10.0f;
	public float xSpeed = 10.0f;
	
	float rotationYAxis = 0.0f;
	
	float velocityX = 0.0f;


	public bool isRotating = false;
	float limitRotationYAxis;
	
	// Use this for initialization
	void Start()
	{
		Vector3 angles = transform.eulerAngles;
		rotationYAxis = angles.y;
		limitRotationYAxis = rotationYAxis + 90;

	}
	
	void Update()
	{
		if (target)
		{
			rotationYAxis += velocityX;
			
			Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
			Quaternion toRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotationYAxis, 0);
			Quaternion rotation = toRotation;

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = rotation;
			transform.position = position;
			
			velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime);

			if(rotationYAxis> limitRotationYAxis){
				velocityX = 0.0f;
				limitRotationYAxis = rotationYAxis + 90;
			}
		}
		
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
