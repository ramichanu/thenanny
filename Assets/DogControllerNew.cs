using UnityEngine;
using System.Collections;

public class DogControllerNew : MonoBehaviour {

	EventDispatcher eventDisp;
	public NavMeshAgent agent;
	bool agentHasPath;
	Vector3 randomPosition;

	void Start () {
		NotificationCenter.DefaultCenter.AddObserver(this, "executeScript");
		agent = GetComponent<NavMeshAgent> ();
		eventDisp = EventDispatcher.DefaultEventDispatcher;

		InvokeRepeating("dogRandomMovementEvent", 0, 5);
	}
	
	// Update is called once per frame
	void Update () {
		randomMovement ();
	}

	void randomMovement() {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			agentHasPath = false;
			agent.ResetPath();
			eventFinishedCallback("startRandomMovement");
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

	void startRandomMovement(){
		setRandomPosition();
	}

	void dogRandomMovementEvent(){
		ArrayList canInterruptBy = new ArrayList();
		
		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("dog_dogWalking");
		methodsToCall.Add("dog_startRandomMovement");
		methodsToCall.Add("dog_dogIdle");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt);
	}

	void eventFinishedCallback(string methodExecuted){
		Hashtable options = new Hashtable ();
		string methodCalled = transform.name + "_" + methodExecuted;
		options.Add ("methodCalled", methodCalled);
		NotificationCenter.DefaultCenter.PostNotification(this, "eventIsFinished", options);
	}

	void executeScript(Notification options){
		
		if(options.data["objectName"].ToString() == transform.name)
		{
			Invoke(options.data["scriptMethod"].ToString(), 0);
		}
	}
}
