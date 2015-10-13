using UnityEngine;
using System.Collections;

public class ChildControllerNew : MonoBehaviour {
	EventDispatcher eventDisp;
	public NavMeshAgent agent;
	bool agentHasPath;
	Vector3 randomPosition;
	Transform target;
	GameObject[] danger;

	// Use this for initialization
	void Start () {
		agentHasPath = false;
		agent = GetComponent<NavMeshAgent> ();
		eventDisp = EventDispatcher.DefaultEventDispatcher;
		NotificationCenter.DefaultCenter.AddObserver(this, "executeScript");

		startChildRandomMovementEvent ();
	}
	
	// Update is called once per frame
	void Update () {
		randomMovement ();
		danger = getDangerElements ();
	}

	void startChildRandomMovementEvent(){
		ArrayList canInterruptBy = new ArrayList();
		
		ArrayList methodsToCall = new ArrayList();
		methodsToCall.Add("child_startChildRandomMovement");
		
		ArrayList methodsAfterInterrupt = new ArrayList();
		ArrayList methodsDisabledUntilEventFinished = new ArrayList();
		
		eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
	}
	
	void startChildRandomMovement(){
		InvokeRepeating("childRandomMovement", 0, 1);
		eventFinishedCallback("startChildRandomMovement");
	}

	void childRandomMovement(){
		if (!agentHasPath) {
			agent.speed = 1.5f;
			playAnimation("child_walking", 2.5f);
			setRandomPosition ();
		}
	}

	void randomMovement() {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && agentHasPath) {
			playAnimation("child_idle", 1.5f);
			agent.speed = 1.5f;
			agentHasPath = false;
			agent.ResetPath();
		}
	}

	void setRandomPosition() {
		int random = Random.Range (0, 3);
		randomPosition = getRandomMeshPosition ();
		if (random > 0 && danger.Length > 0) {
			randomPosition = getDangerPosition ();
		}
		agent.SetDestination (randomPosition);
		agentHasPath = true;
	}

	Vector3 getDangerPosition() {
		int dangerPosition = Random.Range (0, danger.Length);
		GameObject dangerFurniDestination = danger [dangerPosition];
		return dangerFurniDestination.transform.position;
	}

	GameObject[] getDangerElements() {
		GameObject[] brokenGlassElements = GameObject.FindGameObjectsWithTag("brokenGlass");
		GameObject[] fireElements = null;
		
		GameObject fires = GameObject.Find ("fires");
		int fireElementsCount = 0;
		if (fires != null) {
			fireElementsCount = 1;
			fireElements = GameObject.FindGameObjectsWithTag("fire");
			if (fireElements.Length > 0) {
				int fireElementRandom = Random.Range(0, fireElements.Length);
				fireElements = new GameObject[1]{fireElements[fireElementRandom]};
			}
		}
		GameObject[] dangerElements = new GameObject[brokenGlassElements.Length + fireElementsCount];
		
		brokenGlassElements.CopyTo(dangerElements, 0);
		if (fires != null) {
			fireElements.CopyTo(dangerElements, brokenGlassElements.Length);
		}
		
		return dangerElements;
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
