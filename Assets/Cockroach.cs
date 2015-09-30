using UnityEngine;
using System.Collections;

public class Cockroach : MonoBehaviour {

	const int MOVING = 1;
	const int ATTACK = 2;
	const int GOTOPLAYER = 3;
	int state = 1;

	NavMeshAgent agent;
	Vector3 randomPosition;

	// Use this for initialization
	void Awake () {
		agent = GetComponent<NavMeshAgent> ();
		//Invoke ("instanciateMoreCockroach", 5f);
	}
	
	// Update is called once per frame
	void Update () {

		switch(state){
			case MOVING:
			randomMovement();
			break;
			case ATTACK:

			break;
			case GOTOPLAYER:
			 if(isCockroachInPlayerPosition()) {
				state = ATTACK;
			}
			break;
		}
	
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
		NavMeshHit hit;
		NavMesh.SamplePosition(position, out hit, 10f, 1);
		position = hit.position;
		return position;
	}

	public void startCockroachAttack(Vector3 position) {
		state = GOTOPLAYER;
		agent.ResetPath();
		agent.SetDestination(position);

	}
	bool isCockroachInPlayerPosition(){
		if (agent.remainingDistance <= agent.stoppingDistance && agent.pathStatus == NavMeshPathStatus.PathComplete) {
			return true;
		}
		return false;
	}

	void instanciateMoreCockroach(){
		Vector3 randomPosition = getRandomHomePosition ();
		GameObject cockroach = Instantiate(Resources.Load("scenary/cockroach")) as GameObject;
		cockroach.name = "cockroach";
		cockroach.transform.position = randomPosition;
	}


}
