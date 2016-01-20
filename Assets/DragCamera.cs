using UnityEngine;

public class DragCamera : MonoBehaviour
{
	public float dragSpeed = 25;
	
	void Start()
	{
		transform.rotation = Quaternion.Euler(30, 45, 0);
		transform.position = new Vector3 (28, 10, 28);
	}
	void Update()
	{
		if (Input.GetMouseButton(0)) 
		{
			var h = -(Input.GetAxis("Mouse X"));
			var v = -(Input.GetAxis("Mouse Y"));
			
			Vector3 move = new Vector3(h, 0, v) * Time.deltaTime * dragSpeed;
			transform.Translate (move); 
		}
		transform.position = new Vector3 (transform.position.x,
		                                  10,
		                                  transform.position.z); 
	}
}