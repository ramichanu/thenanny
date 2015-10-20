using UnityEngine;
using System.Collections;

public class CockroachNew : EventScript {

	RaycastHit hit;
	NavMeshAgent agent;
	bool hasPath = false;
	bool isThisDestroy = false;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		destinationReachedLogic ();
	}

	void goToPlayer(){
		if (!isThisDestroy) {
			Vector3 playerPosition = GameObject.Find ("player").transform.position;
			agent.SetDestination(playerPosition);
			hasPath = true;
		}
	}

	void destinationReachedLogic () {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && hasPath) {
			hasPath = false;
			//eventFinishedCallback("goToPlayer");
		}
	}

	void OnTriggerEnter(Collider collision) {
		
		switch (collision.transform.name) {
		case "player":
			ArrayList canInterruptBy = new ArrayList();
			ArrayList methodsAfterInterrupt = new ArrayList();
			ArrayList methodsDisabledUntilEventFinished = new ArrayList();
			ArrayList methodsToCall = new ArrayList();

			methodsToCall.Add("player_playNannyCockroach");
			methodsToCall.Add("cockroach_goToPlayer");

			methodsDisabledUntilEventFinished.Add ("player_moveCharacterToClickedDestination");
			eventDisp.addEvent(methodsToCall, canInterruptBy, methodsAfterInterrupt, methodsDisabledUntilEventFinished);
			
			break;
		}
		
	}

	public void destroyCockroach(){
		Destroy (transform.gameObject);
		eventFinishedCallback("destroyCockroach");
		eventFinishedCallback("goToPlayer");
	}

	void OnDisable()
	{
		isThisDestroy = true;
	}


}
