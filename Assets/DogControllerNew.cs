using UnityEngine;
using System.Collections;

public class DogControllerNew : EventScript {
	
	public NavMeshAgent agent;
	bool agentHasPath;
	Vector3 randomPosition;
	Transform target;

	void Start () {
		agent = GetComponent<NavMeshAgent> ();

		target = GameObject.Find ("cat").transform;
		startDogRandomMovementEvent ();

	}
	
	// Update is called once per frame
	void Update () {
		randomMovement ();
	}

	void startDogRandomMovementEvent(){
		ArrayList canInterruptBy = new ArrayList();
		
		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("dog_startDogRandomMovement");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}

	void startDogRandomMovement(){
		InvokeRepeating("dogRandomMovementEvent", 0, 7);
		eventFinishedCallback("startDogRandomMovement");
	}

	void stopDogRandomMovement(){
		agent.ResetPath ();
		CancelInvoke("dogRandomMovementEvent");
		CancelInvoke("dogFollowRunning");

		eventFinishedCallback("stopDogRandomMovement");
	}

	void randomMovement() {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			playAnimation("dog_idle", 1.5f);
			agent.speed = 1.5f;
			agentHasPath = false;
			agent.ResetPath();
		}
	}

	void setRandomPosition() {
		randomPosition = getRandomMeshPosition ();
		agent.SetDestination (randomPosition);
		agentHasPath = true;
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

	void dogWalking(){
		playAnimation("dog_walking", 1.5f);
		eventFinishedCallback("dogWalking");
	}

	void dogIdle(){
		playAnimation("dog_idle", 1.5f);
		eventFinishedCallback("dogIdle");
	}

	void dogRunning(){
		playAnimation("dog_running", 1.5f);
		eventFinishedCallback("dogRunning");
	}
	

	void dogRandomMovementEvent(){
		playAnimation("dog_walking", 1.5f);
		setRandomPosition ();
	}

	void dogFollowRunning(){
		agent.speed = 3f;
		playAnimation("dog_running", 1.5f);
		agent.SetDestination(target.position+target.forward*-0.3f);
	}

	void startDogFollowRunning(){
		InvokeRepeating("dogFollowRunning", 0, 0.01f);
		eventFinishedCallback("startDogFollowRunning");
	}


}
