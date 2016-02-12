using UnityEngine;
using System.Collections;

public class CockroachNew : EventScript {

	RaycastHit hit;
	NavMeshAgent agent;
	bool hasPath = false;
	bool isThisDestroy = false;
	bool isHitPlayer = false;
	Vector3 randomPosition;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();

	}
	
	// Update is called once per frame
	void Update () {
		destinationReachedLogic ();
		if (!isHitPlayer) {
			randomMovement ();
		}

	}



	void destinationReachedLogic () {
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete && hasPath) {
			hasPath = false;
			if(isHitPlayer) {
				Hashtable options = new Hashtable ();
				NotificationCenter.DefaultCenter.PostNotification(this, "cockroachReachsTarget", options);
			}
		}
	}

	void OnTriggerEnter(Collider collision) {
		bool isShortDistance = Vector3.Distance (collision.transform.position, transform.position) < 2f;
		switch (collision.transform.name) {
		case "player":
			if(isShortDistance) {
				if (!GameObject.Find ("cockroachManager").GetComponent<CockroachManager>().isCockroachAnnoying) {
					GameObject.Find ("cockroachManager").GetComponent<CockroachManager>().hit = gameObject;
					GameObject.Find ("cockroachManager").GetComponent<CockroachManager>().gotoPlayerEvent();
					isHitPlayer = true;
				}
			}
			break;
		}
		
	}


	void OnDisable()
	{
		isThisDestroy = true;
	}

	void randomMovement() {
		
		if (agent.remainingDistance <= float.Epsilon && agent.pathStatus == NavMeshPathStatus.PathComplete) {
			agent.ResetPath ();
			setRandomPosition ();
		} else if(agent.hasPath == false) {
			setRandomPosition ();
		}
	}

	void setRandomPosition() {
		randomPosition = getRandomHomePosition ();
		agent.SetDestination (randomPosition);
	}

	Vector3 getRandomHomePosition() {
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

	void stopCockroachMovement(){
		GameObject hit = GameObject.Find ("player").GetComponent<PlayerMovementNew>().hit.transform.gameObject;
		if (gameObject == hit) {
			agent.Stop ();
			agent.ResetPath();
			isHitPlayer = true;
			eventFinishedCallback("stopCockroachMovement");
		}
	}

	public void goToPlayer(){
		if (!isThisDestroy) {
			Vector3 playerPosition = GameObject.Find ("player").transform.position;
			agent.SetDestination(playerPosition);
			hasPath = true;
		}
	}

}
