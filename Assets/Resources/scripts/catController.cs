using UnityEngine;
using System.Collections;

public class catController : MonoBehaviour {
	const int WALK = 0;
	const int STOP = 1;
	const int HUIDA = 2;

	public Transform target;
	public NavMeshAgent agent;
	public bool ignoreDog = false;
	bool agentHasPath;
	Vector3 randomPosition;
	bool isRunning = false;
	public int state = -1;
	GameObject[] dangerFurniDestinations;
	GameObject dangerFurniDestination;

	void Start () {
		agentHasPath = false;
		agent = GetComponent<NavMeshAgent> ();
		setDangerFurniDestinations ();

	}

	void Update () {
		Debug.DrawRay(transform.position, transform.forward*0.3f, Color.blue);
		Debug.DrawRay(transform.position, transform.right*0.3f, Color.blue);
		Debug.DrawRay(transform.position, transform.right*-0.3f, Color.blue);
		buildSphereCast ();

		if (!isRunning) {
			state = Random.Range (0, 2);
		}

		switch (state) {
		case WALK:
			if (!isRunning) {
				playAnimation ("cat_walking", 2f);
				randomMovement ();
			}
			break;
		case STOP:
			if (!agentHasPath && !isRunning) {
				playAnimation ("cat_idle", 1f);
				StartCoroutine(stopFewSeconds());
				StopCoroutine(stopFewSeconds());
			}
			break;
		case HUIDA:
			playAnimation ("cat_running", 3f);
			int stateFollow = 2;
			int targetState = target.GetComponent<dogController>().state;
			if (targetState != stateFollow) {
				targetState = stateFollow;
				target.GetComponent<dogController>().state = 2;
				target.GetComponent<dogController>().isRunning = true;
			}

			isRunning = true;
			randomMovement ();
			break;
		}

	}

	void randomMovement() {
		if (agent.pathStatus == NavMeshPathStatus.PathComplete && !agentHasPath) {
			if(dangerFurniDestination != null){
 				transform.LookAt(dangerFurniDestination.transform);
			}
			agentHasPath = true;
		}
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			agentHasPath = false;
			agent.ResetPath();
			if (state == HUIDA) {
				playAnimation ("cat_running", 3f);
				int randomNum = Random.Range(0, 6);
				if (randomNum > 2) {
					setDangerFurniPosition();
				} else {
					setRandomPosition();
				}
			} else {
				playAnimation ("cat_walking", 2f);
				setRandomPosition();
			}
		}
	}

	void setRandomPosition() {
		randomPosition = getRandomMeshPosition ();
		agent.SetDestination (randomPosition);
	}
	private Vector3 getRandomMeshPosition () {
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
	
	IEnumerator stopFewSeconds(){
		isRunning = true;
		int seconds = Random.Range (2, 5);
		agent.Stop ();
		agentHasPath = false;
		yield return new WaitForSeconds(seconds);
		agent.Resume ();
		isRunning = false;
	}

	void buildSphereCast() {
		CharacterController charCtrl = GetComponent<CharacterController>();
		Vector3 p1 = transform.position + charCtrl.center;
		RaycastHit[] aroundHits = Physics.SphereCastAll (p1, charCtrl.height / 2, transform.forward, 10);
		foreach(RaycastHit aroundHit in aroundHits) {
			if (aroundHit.transform.tag == "dog" && !isRunning && !ignoreDog) {
				launchHuida(aroundHit.transform.gameObject);
			}
		}
		if(state == HUIDA){
			ArrayList hits = new ArrayList();
			hits.Add(Physics.RaycastAll (transform.position, transform.forward*0.5f, 0.3f));
			hits.Add(Physics.RaycastAll (transform.position, transform.right*0.5f, 0.3f));
			hits.Add(Physics.RaycastAll (transform.position, transform.right*-0.5f, 0.3f));
			if (hits.Count > 0) {
				foreach(RaycastHit[] raycastArray in hits){
					foreach(RaycastHit hitItem in raycastArray){
						if ((hitItem.transform.name == "glasses" || hitItem.transform.name == "glasses2")) {
							bool dangerDropped = hitItem.transform.gameObject.GetComponent<dangerFurni>().dangerDropped;
							if(!dangerDropped){
								hitItem.transform.gameObject.GetComponent<dangerFurni>().dangerDropped = true;
								GameObject brokenGlass = Instantiate(Resources.Load("scenary/brokenGlass")) as GameObject;
								brokenGlass.name = "brokenGlass";
								brokenGlass.transform.position = hitItem.transform.position + hitItem.transform.right *0.7f;
								brokenGlass.GetComponent<dangerItem>().parent = hitItem.transform.gameObject;
							}
							
						} else if(hitItem.transform.name == "heater"){
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
	}

	IEnumerator returnToOriginalState(){
		int seconds = Random.Range (8, 12);
		yield return new WaitForSeconds(seconds);
		NavMeshAgent hitAgent = target.gameObject.GetComponent<NavMeshAgent> ();
		hitAgent.speed -= 2;
		agent.speed -= 2;
		isRunning = false;

		StartCoroutine(ignoreDogFewSeconds());
		StopCoroutine(ignoreDogFewSeconds());
	}

	void setDangerFurniDestinations() {
		dangerFurniDestinations = GameObject.FindGameObjectsWithTag("dangerFurni");
	}

	void setDangerFurniPosition(){
		int DangerFurniPosition = Random.Range (0, dangerFurniDestinations.Length);
		dangerFurniDestination = dangerFurniDestinations [DangerFurniPosition];
		agent.SetDestination (dangerFurniDestination.transform.position);

	}

	public void launchHuida(GameObject hitObject){
		state = HUIDA;
		agent.speed += 2;
		NavMeshAgent hitAgent = hitObject.GetComponent<NavMeshAgent> ();
		hitAgent.speed += 2;
		playAnimation ("cat_running", 3f);
		
		StartCoroutine(returnToOriginalState());
		StopCoroutine(returnToOriginalState());
		isRunning = true;
	}

	IEnumerator ignoreDogFewSeconds(){
		ignoreDog = true;
		yield return new WaitForSeconds(10);
		ignoreDog = false;
	}

	public void playAnimation(string animation, float speed){
		gameObject.GetComponent<Animation>()[animation].speed = speed;
		gameObject.GetComponent<Animation>().Play(animation);
	}
}
