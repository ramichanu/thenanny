using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour {
	public bool babbyBottle;

	NavMeshAgent agent;
	bool agentHasPath;
	RaycastHit hit;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		agentHasPath = false;
		babbyBottle = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)){
				switch(hit.transform.tag) {
					case "terrain":
					case "brokenGlass":
					case "child":
					case "fire":
					case "babyBottle":
						agent.SetDestination(new Vector3(hit.point.x, transform.position.y, hit.point.z));
					break;
				}
			}
		}
		destinationReachedLogic ();
	}

	void destinationReachedLogic () {
		if (agent.hasPath && !agentHasPath) {
			agentHasPath = true;
		}
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			//Debug.Log ("You have reached your destination");
			agentHasPath = false;
			if (hit.transform != null) {
				if (hit.transform.tag == "objectToInteract") {
					Destroy(hit.transform.gameObject);
				}
			}
		}
	}

	void OnTriggerEnter(Collider collision) {

		switch (collision.transform.name) {
			case "brokenGlass":
				if (hit.transform.tag == "brokenGlass") {
					collision.gameObject.GetComponent<dangerItem>().parent.GetComponent<dangerFurni>().dangerDropped = false;
					Destroy(collision.transform.gameObject);
				}
				
				break;
			case "babyBottle":
				if (hit.transform.tag == "babyBottle") {
					Destroy(collision.transform.gameObject);
					babbyBottle = true;
				}
			break;

		}
	}

	void OnCollisionEnter(Collision collision) {

		switch (collision.transform.name) {
			case "child":
			if (hit.transform.tag == "child" && babbyBottle) {
				babbyBottle = false;
				GameObject hungerBar = GameObject.Find("hungerBar");
				hungerBar.GetComponent<Image> ().fillAmount = 0;
				hit.transform.gameObject.GetComponent<childController>().hunger = 0;
			}
			break;
		}
	}
}
