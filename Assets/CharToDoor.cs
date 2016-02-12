using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharToDoor : MonoBehaviour {
	public Ray ray;
	public Ray ray2;
	public Ray ray3;
	public RaycastHit hit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


		Debug.DrawRay(transform.position, transform.forward* 0.6f);

		ray = new Ray (transform.position + new Vector3(0, 1, 0), transform.forward);
		ray2 = new Ray (transform.position + new Vector3(0, 1f, 0.1f), transform.forward);
		ray3 = new Ray (transform.position + new Vector3(0, 1f, -0.1f), transform.forward);
		
		
		if (Physics.Raycast(ray, out hit, 0.6f) || Physics.Raycast(ray2, out hit, 0.6f) || Physics.Raycast(ray3, out hit, 0.6f)) {
			switch(hit.transform.tag) {
			case "door":
				//hit.transform.GetComponent<Door>().rotateDoor (transform.gameObject, 90f, false, hit);
				break;
			}
		}
	}
}
