using UnityEngine;
using System.Collections;

public class dangerStrategy : MonoBehaviour {
	const int DANGER_PETS = 1;
	const int DANGER_MADLADY = 2;
	public const int DANGER_LIGHTNING_STORM = 3;
	const int DANGER_ZOO = 4;
	const int DANGER_PHONE = 5;
	const int DANGER_COCKROACH = 6;

	public int childLive;
	public int currentDanger;

	public bool isLightningStormEnabled = false;

	// Use this for initialization
	void Start () {
		childLive = GameObject.Find ("child").GetComponent<childController>().lives;
		InvokeRepeating ("addEventDangers",1,10f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void addEventDangers(){
		int updatedChildLive = GameObject.Find ("child").GetComponent<childController>().lives;
		if (childLive == updatedChildLive) {
			ArrayList dangers = getAvailableDangers();
			currentDanger = (int)dangers[Random.Range(0, dangers.Count)];
			switch(currentDanger){
			case DANGER_PETS:
				launchDangerPets();
				break;
			case DANGER_MADLADY:
				//launchDangerMadLady();
				break;
			case DANGER_LIGHTNING_STORM:
				//launchLightningStorm();
				break;
			case DANGER_COCKROACH:
				//launchDangerCockroach();
				break;
			}
		}
		childLive = updatedChildLive;
	}

	void launchDangerPets(){
		GameObject cat = GameObject.Find ("cat");
		GameObject dog = GameObject.Find ("dog");
		Vector3 position = dog.GetComponent<dogController>().getRandomMeshPosition();
		
		cat.GetComponent<catController>().agent.ResetPath();
		dog.GetComponent<dogController>().agent.ResetPath();

		cat.GetComponent<catController> ().playAnimation ("cat_walking", 2f);
		cat.GetComponent<catController>().agent.SetDestination(position);
		dog.GetComponent<dogController>().agent.SetDestination(position);
	}

	void launchDangerMadLady(){
		Vector3 portalEnterPosition = getPortalScenaryPosition();
		GameObject madLady = Instantiate(Resources.Load("characters/madLady")) as GameObject;
		madLady.name = "madLady";
		madLady.transform.position = portalEnterPosition;
	}

	Vector3 getPortalScenaryPosition(){
		GameObject portalScenary = GameObject.Find ("portalEnter");
		return portalScenary.transform.position;
	}

	ArrayList getAvailableDangers(){
		ArrayList availableDangers = new ArrayList ();
		availableDangers.Add (DANGER_PETS);
		availableDangers.Add (DANGER_COCKROACH);
		if (GameObject.Find ("madLady") == null) {
			availableDangers.Add (DANGER_MADLADY);
		}
		if (GameObject.Find ("lightningStorm") == null) {
			availableDangers.Add (DANGER_LIGHTNING_STORM);
		}



		return availableDangers;
	}
	void launchLightningStorm(){
		isLightningStormEnabled = true;
		Vector3 portalEnterPosition = getPortalScenaryPosition();
		GameObject lightningStorm = Instantiate(Resources.Load("scenary/lightningStorm")) as GameObject;
		lightningStorm.name = "lightningStorm";
		lightningStorm.transform.position = portalEnterPosition;
		Invoke("removeLightingStorm", Random.Range (30, 50));
	}

	void removeLightingStorm(){
		GameObject directionalLight = GameObject.Find ("directionalLight");
		GameObject playerLight = GameObject.Find ("playerLight");
		
		directionalLight.GetComponent<Light> ().intensity = 1;
		playerLight.GetComponent<Light> ().enabled = false;

		isLightningStormEnabled = false;
		Destroy(GameObject.Find ("lightningStorm"));
	}

	void launchDangerCockroach(){
		GameObject dog = GameObject.Find ("dog");
		Vector3 position = dog.GetComponent<dogController>().getRandomMeshPosition();
		GameObject cockroach = Instantiate(Resources.Load("scenary/cockroach")) as GameObject;
		cockroach.name = "cockroach";
		cockroach.transform.position = position;

	}

}
