using UnityEngine;
using System.Collections;

public class HideWalls : MonoBehaviour {
	public Ray ray;
	public RaycastHit hit;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);

		if (Physics.Raycast(ray, out hit)) {
			switch(hit.transform.tag){
				case "wall":
				//Destroy(hit.transform.gameObject);
				break;
			}
		}
	}
}
