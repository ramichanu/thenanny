using UnityEngine;
using System.Collections;

public class dangerStrategy : MonoBehaviour {
	const int DANGER_PETS = 0;
	const int DANGER_CREEPYWOMAN = 0;
	const int DANGER_STORM = 0;
	const int DANGER_ZOO = 0;
	const int DANGER_PHONE = 0;

	public int childLive;
	// Use this for initialization
	void Start () {
		childLive = GameObject.Find ("child").GetComponent<childController>().lives;
		InvokeRepeating ("addEventDangers",3,30f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void addEventDangers(){
		int updatedChildLive = GameObject.Find ("child").GetComponent<childController>().lives;
		if (childLive == updatedChildLive) {
			Debug.Log ("catdogpos");
			GameObject cat = GameObject.Find ("cat");
			GameObject dog = GameObject.Find ("dog");
			Vector3 position = dog.GetComponent<dogController>().getRandomMeshPosition();

			cat.GetComponent<catController>().agent.ResetPath();
			dog.GetComponent<dogController>().agent.ResetPath();

			cat.GetComponent<catController>().agent.SetDestination(position);
			dog.GetComponent<dogController>().agent.SetDestination(position);
		}
	}


}
