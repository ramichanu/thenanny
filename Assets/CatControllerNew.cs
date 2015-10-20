using UnityEngine;
using System.Collections;

public class CatControllerNew : EventScript {
	
	public NavMeshAgent agent;
	bool agentHasPath;
	Vector3 randomPosition;
	public GameObject target;
	GameObject[] dangerFurniDestinations;
	GameObject dangerFurniDestination;

	void Start () {
		agent = GetComponent<NavMeshAgent> ();

		setDangerFurniDestinations ();
		startCatRandomMovementEvent ();
	}
	
	// Update is called once per frame
	void Update () {
		randomMovement ();
	}
	
	void randomMovement() {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			playAnimation("cat_idle", 1.5f);
			agentHasPath = false;
			agent.ResetPath();
		}
	}
	
	void setRandomPosition() {
		randomPosition = getRandomMeshPosition ();
		agent.SetDestination (randomPosition);
		agentHasPath = true;
	}

	void setDangerFurniDestinations() {
		dangerFurniDestinations = GameObject.FindGameObjectsWithTag("dangerFurni");
	}

	void startCatRandomMovementEvent(){
		ArrayList canInterruptBy = new ArrayList();
		
		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("cat_startCatRandomMovement");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}
	
	void startCatRandomMovement(){
		InvokeRepeating("catRandomMovementEvent", 0, 7);
		eventFinishedCallback("startCatRandomMovement");
	}

	void catRandomMovementEvent(){
		if (!agentHasPath) {
			agent.speed = 1.5f;
			playAnimation("cat_walking", 1.5f);
			setRandomPosition ();
		}
	}

	void stopCatRandomMovement(){
		agent.ResetPath ();
		CancelInvoke("catRandomMovementEvent");
		eventFinishedCallback("stopCatRandomMovement");
	}

	void stopCatRunaway(){
		agent.ResetPath ();
		CancelInvoke("catRandomMovementEvent");
		CancelInvoke ("enableCatDangerCollisions");
		CancelInvoke("catRandomMovementToDangers");
		eventFinishedCallback("stopCatRunaway");
	}

	
	public Vector3 getRandomMeshPosition () {
		GameObject terrain = GameObject.FindWithTag ("terrainHome");
		float xTerrainMin = terrain.GetComponent<Renderer>().bounds.min.x;
		float xTerrainMax = terrain.GetComponent<Renderer>().bounds.max.x;
		float zTerrainMin = terrain.GetComponent<Renderer>().bounds.min.z;
		float zTerrainMax = terrain.GetComponent<Renderer>().bounds.max.z;
		Vector3 position = new Vector3(Random.Range(xTerrainMin, xTerrainMax), 0, Random.Range(zTerrainMin, zTerrainMax));
		NavMeshHit hit;
		NavMesh.SamplePosition(position, out hit, 10f, 1);
		position = hit.position;
		return position;
	}
	
	void playAnimation(string animation, float speed){
		gameObject.GetComponent<Animation>()[animation].speed = speed;
		gameObject.GetComponent<Animation>().Play(animation);
	}
	
	void catWalking(){
		playAnimation("cat_walking", 1.5f);
		eventFinishedCallback("catWalking");
	}
	
	void catIdle(){
		playAnimation("cat_idle", 1.5f);
		eventFinishedCallback("catIdle");
	}

	void startCatRunaway(){
		InvokeRepeating ("enableCatDangerCollisions", 0, 0.01f);
		InvokeRepeating("catRandomMovementToDangers", 0, 0.01f);
		eventFinishedCallback("startCatRunaway");
	}

	void catRandomMovementToDangers(){
		if (!agentHasPath) {
			playAnimation ("cat_running", 3f);
			agent.speed = 3.3f;
			//agent.SetDestination(target.transform.position+transform.forward*-0.3f);
			setDangerFurniPosition ();
		}
	}

	void setDangerFurniPosition(){
		int DangerFurniPosition = Random.Range (0, dangerFurniDestinations.Length);
		dangerFurniDestination = dangerFurniDestinations [DangerFurniPosition];

		bool dangerDropped = false;
		if (transform.tag == "shelf") {
			dangerDropped = dangerFurniDestination.transform.parent.gameObject.GetComponent<dangerFurni>().dangerDropped;
		} else {
			dangerDropped = dangerFurniDestination.GetComponent<dangerFurni> ().dangerDropped;
		}

		if (!dangerDropped) {
			agent.SetDestination (dangerFurniDestination.transform.position+transform.forward*-0.3f);
		} else {
			//agent.SetDestination (dangerFurniDestination.transform.position+transform.forward*-0.3f);
			agent.SetDestination (getRandomMeshPosition());
		}

		agentHasPath = true;
	}

	void enableCatDangerCollisions() {
		CharacterController charCtrl = GetComponent<CharacterController>();
		Vector3 p1 = transform.position + charCtrl.center;
		RaycastHit[] aroundHits = Physics.SphereCastAll (p1, charCtrl.height / 2, transform.forward, 10);

		ArrayList hits = new ArrayList();
		hits.Add(Physics.RaycastAll (transform.position, transform.forward*0.5f, 0.3f));
		hits.Add(Physics.RaycastAll (transform.position, transform.right*0.5f, 0.3f));
		hits.Add(Physics.RaycastAll (transform.position, transform.right*-0.5f, 0.3f));
		if (hits.Count > 0) {
			foreach(RaycastHit[] raycastArray in hits){
				foreach(RaycastHit hitItem in raycastArray){
					if ((hitItem.transform.name == "shelf" || hitItem.transform.name == "shelf2")) {
						
						bool dangerDropped = hitItem.transform.gameObject.GetComponent<dangerFurni>().dangerDropped;
						if(!dangerDropped){
							hitItem.transform.gameObject.GetComponentInChildren<Animation>()["shelf_shaking"].speed = 1;
							hitItem.transform.gameObject.GetComponentInChildren<Animation>().Play("shelf_shaking");
							hitItem.transform.gameObject.GetComponentInChildren<dangerFurni>().dangerDropped = true;
							StartCoroutine(instantiateBrokenGlass(hitItem.transform.gameObject));
						}
						
					}
					else if ((hitItem.transform.name == "jartable")) {
						
						bool dangerDropped = hitItem.transform.gameObject.GetComponent<dangerFurni>().dangerDropped;
						if(!dangerDropped){
							hitItem.transform.gameObject.GetComponent<Animation>()["jartable_anim"].speed = 1;
							hitItem.transform.gameObject.GetComponent<Animation>().Play("jartable_anim");
							hitItem.transform.gameObject.GetComponent<dangerFurni>().dangerDropped = true;
							StartCoroutine(instantiateBrokenJar(hitItem.transform.gameObject));
						}
						
					}
					else if ((hitItem.transform.name == "tvTable")) {
						
						bool dangerDropped = hitItem.transform.gameObject.GetComponent<dangerFurni>().dangerDropped;
						if(!dangerDropped){
							hitItem.transform.gameObject.GetComponent<Animation>()["tv_falling"].speed = 1;
							hitItem.transform.gameObject.GetComponent<Animation>().Play("tv_falling");
							hitItem.transform.gameObject.GetComponent<dangerFurni>().dangerDropped = true;
							ParticleSystem electricity = GameObject.Find("electricity").GetComponent<ParticleSystem>();
							electricity.Play();
							
						}
						
					}
					else if(hitItem.transform.name == "heater"){
						bool dangerDropped = hitItem.transform.gameObject.GetComponent<dangerFurni>().dangerDropped;
						if(!dangerDropped){
							hitItem.transform.gameObject.GetComponent<dangerFurni>().dangerDropped = true;
							GameObject fires = Instantiate(Resources.Load("scenary/fires")) as GameObject;
							fires.name = "fires";
							Vector3 firePosition = hitItem.transform.position + hitItem.transform.right *0.7f;
							StartCoroutine(fires.GetComponent<fires>().addFire(firePosition));
							fires.GetComponent<fires>().startFireBehaviour();
						}
					}
				}
			}
		}

	}

	IEnumerator instantiateBrokenGlass(GameObject hitItem){
		yield return new WaitForSeconds (0.8f);
		GameObject brokenGlass = Instantiate(Resources.Load("scenary/brokenGlass")) as GameObject;
		brokenGlass.name = "brokenGlass";
		brokenGlass.transform.position = hitItem.transform.position + hitItem.transform.right * 0.7f;
		brokenGlass.GetComponent<dangerItem>().parent = hitItem.transform.gameObject;
		
	}
	IEnumerator instantiateBrokenJar(GameObject hitItem){
		yield return new WaitForSeconds (0.5f);
		GameObject brokenJar = Instantiate(Resources.Load("scenary/brokenJar")) as GameObject;
		brokenJar.name = "brokenJar";
		brokenJar.transform.position = hitItem.transform.position + hitItem.transform.right * 0.7f;
		brokenJar.GetComponent<dangerItem>().parent = hitItem.transform.gameObject;
		
	}


}
