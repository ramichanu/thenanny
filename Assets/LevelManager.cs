using UnityEngine;
using System.Collections;

public class LevelManager : EventScript {

	int randomTimeToStart;
	bool isMadLadyInvoke = false;
	// Use this for initialization
	void Start () {
		//Invoke ("startCatAndDogPersecution", 2f);
		//Invoke ("launchDangerMadLady", 2f);
		startLevel1 ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void startLevel1() {

		InvokeRepeating ("startCatAndDogIA", 5, 15);
		InvokeRepeating ("startDangerMadLadyIA", 0, 15);
		//InvokeRepeating ("startDangerCockroach", 15, 50);
		//InvokeRepeating ("startLightningStorm", 15, 50);
	}

	void startCatAndDogIA() {
		startCatAndDogPersecution ();
		int timeToStopPersecution = Random.Range (5, 10);
		Invoke ("stopCatAndDogPersecution", timeToStopPersecution);
	}

	void startDangerCockroach() {
		int randomTime = Random.Range (0, 20);
		Invoke ("launchDangerCockroach", randomTime);
	}

	void startLightningStorm() {
		int randomTime = Random.Range (0, 3);
		Invoke ("launchLightningStorm", randomTime);
	}

	void startDangerMadLadyIA() {
		if (!isMadLadyInvoke) {
			isMadLadyInvoke = true;
			int randomTime = Random.Range (15, 50);
			Invoke ("dangerMadLadyIA", randomTime);
		}
	}

	void startCatAndDogPersecution() {
		ArrayList canInterruptBy = new ArrayList();
		ArrayList methodsToCall = new ArrayList();
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();

		methodsToCall.Add("cat_stopCatRandomMovement");
		methodsToCall.Add("dog_stopDogRandomMovement");
		methodsToCall.Add("cat_startCatRunaway");
		methodsToCall.Add("dog_startDogFollowRunning");

		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void stopCatAndDogPersecution (){
		ArrayList canInterruptBy = new ArrayList();

		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("cat_stopCatRunaway");
		methodsToCall.Add("dog_stopDogRandomMovement");
		methodsToCall.Add("cat_startCatRandomMovement");
		methodsToCall.Add("dog_startDogRandomMovement");

		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void cockroachPlague() {

	}

	void madLady(){
		ArrayList canInterruptBy = new ArrayList();
		
		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add ("madLady_enableAgentEvent");
		methodsToCall.Add("madLady_startFollowChildEvent");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void dangerMadLadyIA() {
		isMadLadyInvoke = false;
		GameObject madLady = GameObject.Find ("madLady");

		if (madLady == null) {
			Invoke ("launchDangerMadLady", 2f);
		}
	}

	void launchDangerMadLady(){

		GameObject.Find ("AlertDangerSystem").GetComponent<DangerAlertSystem> ().addDangerAlerts ("madLady");

		Vector3 portalEnterPosition = getPortalScenaryPosition();
		GameObject madLady = Instantiate(Resources.Load("characters/madLady")) as GameObject;
		madLady.name = "madLady";
		madLady.transform.position = portalEnterPosition;

		this.madLady ();
	}
	
	Vector3 getPortalScenaryPosition(){
		GameObject portalScenary = GameObject.Find ("portalEnter");
		return portalScenary.transform.position;
	}

	void launchDangerCockroach(){
		GameObject[] cockroachs = GameObject.FindGameObjectsWithTag ("cockroach");
		if (cockroachs.Length == 0) {
			Vector3 position = getRandomMeshPosition();
			GameObject cockroachManager = Instantiate(Resources.Load("scenary/cockroachManager")) as GameObject;
			cockroachManager.name = "cockroachManager";
		}
	}

	public Vector3 getRandomMeshPosition () {
		GameObject terrain = GameObject.FindWithTag ("terrainHome");
		float xTerrainMin = terrain.GetComponent<Renderer>().bounds.min.x;
		float xTerrainMax = terrain.GetComponent<Renderer>().bounds.max.x;
		float zTerrainMin = terrain.GetComponent<Renderer>().bounds.min.z;
		float zTerrainMax = terrain.GetComponent<Renderer>().bounds.max.z;
		Vector3 position = new Vector3(Random.Range(xTerrainMin, xTerrainMax), 0, Random.Range(zTerrainMin, zTerrainMax));
		NavMeshHit hit2;
		NavMesh.SamplePosition(position, out hit2, 10f, 1);
		position = hit2.position;
		return position;
	}

	void launchLightningStorm(){

		Vector3 portalEnterPosition = getPortalScenaryPosition();
		GameObject lightningStorm = Instantiate(Resources.Load("scenary/lightningStorm")) as GameObject;
		lightningStorm.name = "lightningStorm";
		lightningStorm.transform.position = portalEnterPosition;
		Invoke("removeLightingStorm", Random.Range (30, 50));
	}

	void removeLightingStorm(){
		GameObject directionalLight = GameObject.Find ("directionalLight");

		directionalLight.GetComponent<Light> ().intensity = 1;
		Destroy(GameObject.Find ("lightningStorm"));
	}
}
