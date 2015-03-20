using UnityEngine;
using System.Collections;

public class catController : MonoBehaviour {
	const int WALK = 0;
	const int STOP = 1;
	const int HUIDA = 2;

	public Transform target;
	NavMeshAgent agent;
	bool agentHasPath;
	Vector3 randomPosition;
	bool isRunning = false;
	int state = -1;
	GameObject[] dangerFurniDestinations;
	GameObject dangerFurniDestination;

	void Start () {
		agentHasPath = false;
		agent = GetComponent<NavMeshAgent> ();
		setDangerFurniDestinations ();
	}

	void Update () {
		Debug.DrawRay(transform.position, transform.forward*0.5f, Color.blue);
		Debug.DrawRay(transform.position, transform.right*0.5f, Color.blue);
		Debug.DrawRay(transform.position, transform.right*-0.5f, Color.blue);
		buildSphereCast ();

		if (!isRunning) {
			state = Random.Range (0, 2);
		}

		switch (state) {
		case WALK:
			if (!isRunning) {
				randomMovement ();
			}
			break;
		case STOP:
			if (!agentHasPath && !isRunning) {
				StartCoroutine(stopFewSeconds());
				StopCoroutine(stopFewSeconds());
			}
			break;
		case HUIDA:
			int stateFollow = 2;
			if (target.GetComponent<dogController>().state != stateFollow) {
				target.GetComponent<dogController>().state = stateFollow;
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
				int randomNum = Random.Range(0, 3);
				if (randomNum > 0) {
					setDangerFurniPosition();
				} else {
					setRandomPosition();
				}
			} else {
				setRandomPosition();
			}
		}
	}

	void setRandomPosition() {
		randomPosition = getRandomMeshPosition ();
		agent.SetDestination (randomPosition);
	}
	private Vector3 getRandomMeshPosition () {
		GameObject terrain = GameObject.FindWithTag ("terrain");
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
		RaycastHit hit;
		CharacterController charCtrl = GetComponent<CharacterController>();
		Vector3 p1 = transform.position + charCtrl.center;
		if (Physics.SphereCast (p1, charCtrl.height/2, transform.forward, out hit, 10)) {
			if (hit.transform.tag == "dog" && !isRunning) {
				state = HUIDA;
				agent.speed += 2;
				NavMeshAgent hitAgent = hit.transform.gameObject.GetComponent<NavMeshAgent> ();
				hitAgent.speed += 2;

				StartCoroutine(returnToOriginalState());
				StopCoroutine(returnToOriginalState());
				isRunning = true;
			}
		}else if(state == HUIDA){
			if (Physics.Raycast (transform.position, transform.forward*0.5f, out hit, 1)
			    || Physics.Raycast (transform.position, transform.right*0.5f, out hit, 1)
			    || Physics.Raycast (transform.position, transform.right*-0.5f, out hit, 1)) {

				if ((hit.transform.name == "glasses" || hit.transform.name == "glasses2")) {

					bool dangerDropped = hit.transform.gameObject.GetComponent<dangerFurni>().dangerDropped;
					if(!dangerDropped){
						hit.transform.gameObject.GetComponent<dangerFurni>().dangerDropped = true;
						GameObject brokenGlass = Instantiate(Resources.Load("scenary/brokenGlass")) as GameObject;
						brokenGlass.name = "brokenGlass";
						brokenGlass.transform.position = hit.transform.position + hit.transform.right *0.7f;
						brokenGlass.GetComponent<dangerItem>().parent = hit.transform.gameObject;
					}

				} else if(hit.transform.name == "heater"){
					bool dangerDropped = hit.transform.gameObject.GetComponent<dangerFurni>().dangerDropped;
					if(!dangerDropped){
						hit.transform.gameObject.GetComponent<dangerFurni>().dangerDropped = true;
						GameObject fire = Instantiate(Resources.Load("scenary/fire")) as GameObject;
						fire.name = "fire";
						fire.transform.position = hit.transform.position + hit.transform.right *0.7f;
						fire.GetComponent<dangerItem>().parent = hit.transform.gameObject;
					}
				}
			}
		}
	}

	IEnumerator returnToOriginalState(){
		int seconds = Random.Range (10, 15);
		yield return new WaitForSeconds(seconds);
		NavMeshAgent hitAgent = target.gameObject.GetComponent<NavMeshAgent> ();
		hitAgent.speed -= 2;
		agent.speed -= 2;
		isRunning = false;
	}

	void setDangerFurniDestinations() {
		dangerFurniDestinations = GameObject.FindGameObjectsWithTag("dangerFurni");
	}

	void setDangerFurniPosition(){
		int DangerFurniPosition = Random.Range (0, dangerFurniDestinations.Length);
		dangerFurniDestination = dangerFurniDestinations [DangerFurniPosition];
		agent.SetDestination (dangerFurniDestination.transform.position);

	}
}
